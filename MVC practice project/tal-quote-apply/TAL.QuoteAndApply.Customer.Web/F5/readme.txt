F5HealthMonitor.aspx -> 
F5 pings this constantly to ensure that the site is running correctly. It is a .aspx so that the IIS pipeline deals with the request.
This file needs the exact string "Health Check OK" for the server to be included in the load balancer
