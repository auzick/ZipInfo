# ZipInfo
Zip code data provider for Sitecore. Automatically loads data from a CSV file into a MongoDB database, and provies a caching layer.

## Overview

It's a fairly common need zip code data in a Sitecore project. Perhaps you need the coordinates of a zip code to put a pin in a map, perhaps to get time zone information for a zip code (especially since the current version of Sitecore's GeoIP does not provide time zone).

This solution maintains a database of fairly lightweight zip code data. It contains timezone offset, DST participation, city and state, and latitude and longitude. If you have access to more robost data, you can replace (or override) the default provider.

For performance, the module uses a Sitecore cache to store queries from the provider. 

## Installing in Sitecore
Install the update package in your Sitecore application. When Sitecore starts, it will populate a MongoDB database with data from a provided zip code document located in App_Data. This file is sourced from [https://boutell.com/zipcodes/](https://boutell.com/zipcodes/).

Whenever this file is updated, it will be reloaded the next time Sitecore starts.

Add a connection string to your ConnectionStrings.config file with the name you'd like to use for your Mongo database. For example

	<add name="zipinfo" connectionString="mongodb://localhost:27017/zipinfo" />

If you want to use a separate mongo database for each Sitecore instance shating a common Mongo server, change the connectionString e.g. `connectionString="mongodb://localhost:27017/myapp_zipinfo"`

### Admin page

There is a administration page at /sitecore/admin/zipinfo.aspx. This page will will allow you to:

- Reload the data manually:
- Query the database
- View the cache size and clear the cache.

## Using from code
Most of what you will need is in static methods in the ZipInfo.ZipInfoManager class.

### Data access methods

- `IEnumerable<IZipCode> GetAll()`

	Returns all of the zip code records in the database

- `IZipCode Get(int zipCode)`
	
	Returns a single record for the specified zip code.

	When querying from the standard provider, to get access to all the fields, cast the resulting data to the provider's poco e.g. 

			var zip = (ZipInfo.Providers.Mongo.ZipCode)ZipInfoManager.Get(12345);

- `bool Set(IZipCode zipCode)`

	Updates a single record

- `string Reload(bool force);`

	Reloads the zip code data. If `false`, the `force` attribute shoud only reload the data if it has changed since the last load.

### Cache management methods

- `bool IsCached(int zipCode)`

	Checks if a record is in the cache

- `long GetCacheSize()`

	Gets the total size (in bytes) of the cache
	
- `int GetCacheCount()`

	Gets the total records in the cache

- `void ClearCache()`

	Flushes the cache


### Cache configuration
The cache is configured in the `ZipInfo.config` file under `sitecore/zipInfo/cache`. The settings are 

- `maxSize`

	This specifies the maximum amount of memory allocated to this cache.

- `objectSize`

	This estimates the amount of memory that each cache record will consume (this is required for Sitecore caches). The default value estimates teh size of the POCO record implemented by the default provider.

## Data file
The default provider expects a csv file, which is available from [https://boutell.com/zipcodes/](https://boutell.com/zipcodes/). The update package will place a copy in your App_Data folder.

If you want to move or rename the data file, there is a setting in the `zipinfo.config` file called `sitecore/zipInfo/provider/zipDataFile`. This value can be...

- An absolute path
- The name of a file in the App_Data folder
- The name of a file on the Sitecore data folder.

## The Data Provider
The module is designed to allow a swappable data provider. The data provider is responsible for...

- Loading data from an external source
- Storing and querying the data.

### The default provider

The module ships with a MongoDB-based data provider. This provider...

- Stores data in a MongoDB database
- Imports data from a basic zip code database from [https://boutell.com/zipcodes/](https://boutell.com/zipcodes/), which the update package will place in the App_Data folder.

####Overriding the default provider

You might have more detailed zip data available to you in CSV format. If you are satisfied with using Mongo as the storage mechanism, but would like to have access to the more datiled data,
you could create a class that inherits from the default provider.

First, create a POCO class that implements `ZipInfo.Providers.IZipCode` that contains all the fields you want to store. Then, create a class that inherits from `ZipInfo.Providers.Mongo.Provider`, and override the `LoadRecord` method. 

        protected IZipCode LoadRecord(List<string> fields)
        {
            var zip = new ZipCode
            {
                Zip = GetInt(fields[0]),
                City = fields[1],
                State = fields[2],
                Latitude = GetFloat(fields[3]),
                Longitude = GetFloat(fields[4]),
                TimezoneOffset = GetInt(fields[5]),
                ParticipatesInDst = GetBool(fields[6])
            };
            return zip;
        }

Simply add mappings for your fields.

In another scenerio, you might be again be satisfied with the MongoDB for storage, but need a different mechanism for importing the data (perhaps through a web service). In that case, you'd override the `Load` method. There you can
do whatever is necessary to import the zip code data into mongo. The methods in the base class are mostly protected, so you still have access to the `Set` method to insert your records as you import them.

#### Replacing the provider
You can replace the default provider by

1. Implementing a provider that inherits the `IZipInfoProvider` interface.
2. Creating a POCO object that inherits from `IZipCode`.
3. Specifying your provider in the `ZipCodes.config` file under `sitecore/zipInfo/provider/zipDataFile/provider`

Your provider must implement the following:

- `IEnumerable<IZipCode> GetAll()`

	Returns all of the zip code records in the database

- `IZipCode Get(int zipCode)`
	
	Returns a single record for the specified zip code

- `bool Set(IZipCode zipCode)`

	Updates a single record

- `string Reload(bool force);`

	Reloads the zip code data. If `false`, the `force` attribute shoud only reload the data if it has changed since the last load.

How the data is stored, queried and loaded is up to the provider.
