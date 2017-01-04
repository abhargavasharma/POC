There should be 2 files in this folder. 

F5HealthMonitor.aspx -> F5 pings this constantly to ensure that the site is running correctly. It is a .aspx so that the IIS pipeline deals with the request.

status.html -> 	F5 pings this to see if the site is Online/Offline. When it is offline F5 will show a maintenance page. 
		It is .html so we can switch between Online/Offline without affecting the app pool. 
		There is a script in Octopus that switches between online/offline.