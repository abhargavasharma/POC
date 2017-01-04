# TAL New Business

## Developer requirements
* All of these can be installed through chef
* .NET 4.6 framework installed (either install VS2015 or install files from http://www.microsoft.com/en-us/download/details.aspx?id=48136)

## Setup
* Clone repo to c:\ilsource\tal-quote-apply
* Open solution in Visual Studio 2015
* Open powershell cmd as admin, run: c:\ilsource\tal-quote-apply\build\build.ps1
	* This may fail on deleting Lucene indexes the first time because the app pool does not exist yet

* Open bash, navigate to: c:\ilsource\tal-quote-apply\TAL.QuoteAndApply.SalesPortal.Web
* Make sure you have grunt, grunt-cli and bower installed globally
* Run: npm install 
	* or copy \\itstorage\Repositories\jspackages\tal-quote-apply\SalesPortal\node_modules to c:\ilsource\tal-quote-apply\TAL.QuoteAndApply.SalesPortal.Web\node_modules
* Run: bower install 
	* or copy \\itstorage\Repositories\jspackages\tal-quote-apply\SalesPortal\bower_components to c:\ilsource\tal-quote-apply\TAL.QuoteAndApply.SalesPortal.Web\bower_components
* Run: grunt

* Open bash, navigate to: c:\ilsource\tal-quote-apply\TAL.QuoteAndApply.Customer.Web
* Make sure you have grunt, grunt-cli and bower installed globally
* Run: npm install 
	* or copy \\itstorage\Repositories\jspackages\tal-quote-apply\Customer\node_module.zip to c:\ilsource\tal-quote-apply\TAL.QuoteAndApply.Customer.Web\node_modules and extract the contents
* Run: bower install 
	* or copy \\itstorage\Repositories\jspackages\tal-quote-apply\Customer\bower_components to c:\ilsource\tal-quote-apply\TAL.QuoteAndApply.Customer.Web\bower_components
* Run: grunt

* Open powershell cmd as admin, run: c:\ilsource\tal-quote-apply\build\configure_local_iis.ps1 to setup IIS
* Open powershell cmd as admin, run: c:\ilsource\tal-quote-apply\build\build.ps1 again

* Navigate to: http://local-talsalesportal.delivery.lan/ (for dev version)
* Navigate to: http://local-talsalesportal-dist.delivery.lan/ (for built dist version)
* Navigate to: http://local-talcustomer.delivery.lan/ (for dev version)
* Navigate to: http://local-talcustomer-dist.delivery.lan/ (for built dist version)


## Build script parameters
* Can be run with switches to run only certain tasks:
* Example: .\build.ps1 -Clean $true -Build $true -SQL $true -Lucene $true
* By default all switches are true

## Angular Component scaffolding
To automagically create angular components when developing the client-side app, setup the following:

1. I’m assuming you have yeoman installed globally, if not then npm install it
2. When you pull down from the CUE repo you should get a /TAL.QuoteAndApply.SalesPortal.Web/node_modules/generator-salesportal’ folder
3. From command prompt, change to the generator-salesportal folder and run npm install 
4. In command prompt, change back to TAL.QuoteAndApply.SalesPortal.Web folder and you should be able to run ‘yo salesportal:component nameOfComponent‘
5. Either enter a full path or press enter for the default (client/app/components)
6. This will create the folder with a js and html template file, then will automatically run the grunt build
7. Add it to the solution 
8. Then just chuck your directive into your page (you’ll need to add the tal prefix) like <tal-name-of-component> and see the awesomeness of the default template!

## CI/DEV/QA Setup
* From DevCloud create new Win2012R2 server with chef client
* Attach to delivery domain
* Run chef-client to register the node
* Create a snapshot in DevCloud
* For a Web server 
  * chef-client -r tal_environment_quoteandapply_webserver@0.1.4
  * You will be required to do a restart one or more times. 
  * After each restart run chef-client -r tal_environment_quoteandapply_webserver@0.1.4 again until it says everything is installed
* For a DB server 
  * chef-client -r tal_sqlserver_2012::install_enterprise_standard@0.4.9 
  * chef-client -r tal_sqlserver_2012::install_update_latest@0.4.9
  
## Okta Token Encryption
We store the Okta api token as an encrypted string in a web.config key. To encrypt the api token:
* You'll be provided with the unencrypted key
* Go to TAL.QuoteAndApply.Infrastructure.Tests > SecurityServiceTests and find Encrypt() test
* Replace string in the test with the unencrypted api token
* Run the test, the encrypted token will be displayed in the console
* Use this encrypted token as the Okta.EncryptedApiToken value in web.config


