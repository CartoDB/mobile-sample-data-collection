## DATA COLLECTION

Sample app demonstrating how **Carto Mobile SDK** and **CARTO SQL API** can be used to actively (and passively) collect data.

The user takes a picture, adds a title, description and, if necessary, a custom identifier, the data is then stored locally (SQLite) until it can be uploaded to CARTO via an SQL function, so it could be realized without the use of your API KEY, which introduces a security risk, if stored on your application.

* Images are stored in Amazon S3 Bucket, only the URL is uploaded to CARTO.
* The function which receives your post request is available in the root folder of this project, **cdb_insert_collected_data**

This application will not run out-of-the-box. You need to provide your own aws access key and secret key via `s3_tokens.txt` file. Our token asset is not included in the repository.

###### SETUP

* Create an account at [carto.com](https://carto.com/)
* Set up a new data set:
![Datasets -> New Dataset](images/image_dataset.png)
* Make the data set available via link (this and the following step are necessary because you shouldn't use CARTO API key in your mobile application)
* Create an SQL function, we recommend via [CDB Manager](https://github.com/CartoDB/cdb-manager) (our function is available [here](cdb_insert_collected_data))
* Create your map
* Grab the map's name from your url: ![](images/image_map_name.png) The GUID at the end of the url is your map's name. CartoMobileSDK retrieves maps by their name. However, the formatting is a bit different: add a `tpl_` prefix and replace dashes with underscores, so with our map, the final format would be: `tpl_76370647_9649_4d19_a6d5_4144348a6f67`

To modify how your data is displayed, click on the layer: ![](images/image_click_layer.png)

Here you can change the color of your entries, how they are aggregated etc.

##### DISCLAIMER

This is a sample app with relatively simple code structure, it has not passed extensive testing etc. I can assure you, if you wish to break it, you will be able to.