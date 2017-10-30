## DATA COLLECTION

Sample app demonstrating how **Carto Mobile SDK** and **CARTO SQL API** can be used to actively (and passively) collect data.

The user takes a picture, adds a title, description and, if necessary, a custom identifier, the data is then stored locally (SQLite) until it can be uploaded to CARTO via an SQL function, so it could be realized without the use of your API KEY, which introduces a security risk, if stored on your application.

* Images are stored in Amazon S3 Bucket, only the URL is uploaded to CARTO.
* The function which receives your post request is available in the root folder of this project, **cdb_insert_collected_data**

This application will not run out-of-the-box. You need to provide your own aws access key and secret key via `s3_tokens.txt` file. Our token asset is not included in the repository.

##### DISCLAIMER

This is a sample app with relatively simple code structure, it has not passed extensive testing etc. I can assure you, if you wish to break it, you will be able to.