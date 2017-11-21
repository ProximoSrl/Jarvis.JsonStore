# Json Store

A minimal microservice to store json payload, with simple funcionalities such as deduplication.

## API

### PUT

It uses simple rest APIS, basically you store a json object with a simple PUT call

```
PUT /api/store/test/8 HTTP/1.1
Host: localhost:40000
Content-Type: application/json
Cache-Control: no-cache
Postman-Token: 29436847-1577-116d-a590-e31400f2af3e
{
    "test" : "Sono 8"
   
}
```

The url is composed by

baseAddress/api/store/{objecttype}/{objectid}

Type can be whathever string you want, it is used only to partitionate data. No mapping or type is associated with the objecttype

It returns a wrapper object with the hash of json payload, **it returns null if the json payload is not changed from the latest version**

### GET

You can retrieve last version of the object with this call

http://localhost:40000/api/store/test/8

Various part of the url are the same of PUT operation.

It returns the object along with hash and other informations.