# GLPI.NET
**GLPI.NET** is .NET GLPI API library which is incredible easy to use thanks to C# dynamic objects.

Usage:
```
//new glpi( "basicauthuser", "basicauthpassword", "app-token", "user-token", "rest url)
glpi glpiConnection = new glpi("finalbeta", "mypassword", "r2jwt47qy39uqt70qbgv0logcjp6207taqq5txurz", null, "https://yourglpi.domain.eu/glpi/apirest.php/"); //Using basic authorization.
//glpi glpiConnection = new glpi(null, null, "r2jwt47qy39uqt70qbgv0logcjp6207taqq5txurz", "r3jwt89qy54uqt74qbgv0logcjp6207taqq5txurz", "https://yourglpi.domain.eu/glpi/apirest.php/"); //Using basic authorization.
glpiConnection.Login();
string resultStr = glpiConnection.jsonResponse("getActiveEntities", null); // returns a json string
ExpandoObject resultObj = glpiConnection.objectResponse("getActiveEntities", null); //returns ExpandoObject. The result in under the property result.
glpiConnection.Logout();
```


Example (List all entities, for each entity, list some details and create a new entiry):
```
glpi glpiConnection = new glpi(null, null, "r2jwt47qy39uqt70qbgv0logcjp6207taqq5txurz", "r3jwt89qy54uqt74qbgv0logcjp6207taqq5txurz", "https://yourglpi.domain.eu/glpi/apirest.php/");
glpiConnection.login();
ExpandoObject responseObj = glpiConnection.objectResponse("getActiveEntities", null);
foreach (dynamic data in ((dynamic)responseObj).result.active_entity.active_entities)
{
   IDictionary<string, object> propertyValues = (IDictionary<string, object>)data;
   foreach (var item in propertyValues)
   {
       ExpandoObject responseObjForEntity = glpiConnection.objectResponse("Entity/" + item.Value , null);
       IDictionary<string, object> propertyValues2 = (IDictionary<string, object>)((dynamic)responseObjForEntity).result;
       foreach (var item2 in propertyValues2)
       {
           if (item2.Key.Equals("name", StringComparison.Ordinal)) Console.WriteLine("{0} => {1}", item2.Key, item2.Value);
           if (item2.Key.Equals("tag", StringComparison.Ordinal)) Console.WriteLine("{0} => {1}", item2.Key, item2.Value);
       }
   }
}
ExpandoObject responseObj = glpiConnection.objectResponse("Entity", new
	{ input = new Object[] { new Dictionary<String, Object> { { "name", "mytest2" }, { "entities_id", 0 }, { "comment", "Created through API" }, { "address", "mystreet 15" }, { "postcode", 1234 }, { "town" , "mytown" }, { "country" , "North Pole" }, { "tag" , 1234 } }
}}); //Create a new entity
glpiConnection.logout();
```

Example using GLPI.NET from Powershell:
```
[Reflection.Assembly]::LoadFile("C:\pathtodll\Newtonsoft.Json.dll");
[Reflection.Assembly]::LoadFile("C:\pathtodll\GLPIApi.dll");
$GLPI = New-Object "GLPIApi.glpi" ([NullString]::Value, [NullString]::Value, $glpi_app_token, $glpi_user_token, $glpi_api_url);
$GLPI.login();
$responseObj = $GLPI.objectResponse("getActiveEntities", $null)
$existingEntities = @{}
foreach ( $data in $responseObj.result.active_entity.active_entities) {
   foreach ( $item in $data) {
       $entityDetails = $GLPI.objectResponse("Entity/$($item.Value)", $null)
       $entityDetails.result.tag
       $entityDetails.result.name
   }
}
$GLPI.logout();
```

> **Note:**

> - GLPI allows a user to log in using basic auth or using an API token. Both methods are supported, but you shouldn't use both at the same time. Send null as a parameter for the method you don't require. You need to use login() method after creating GLPI class and logout() after you are done with the API to make sure there are no open sessions left. 
> - GLPI.NET uses Newtonsoft.Json library for json serialization and deserialization
> - .NET 4.0, but you can compile to any version of .NET as long as it supports ExpandoObject and Newtonsoft.Json 
> - Some GLPI api calls should be get or post. If you supply an ExpandoObject as a second parameter, it will be converted to json and it will be a post. if you supply null, it will be a get request. 