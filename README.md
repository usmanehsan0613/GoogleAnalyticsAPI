# GoogleAnalyticsAPI
To pull google analytics API such as pageviews, userviews and userssessions from google API and to show on custom web pages 

- index.html file sends an ajax request to FetchData [WebMethod] 
- change the settings in Report.aspx.cs Google API crdentials. To update service-account-credentials.json file in files/ directory
- [WebMethod] will pick the google analytics metrics such as pageviews, userviews and usersessions 
- Custom KPI's can be pulled by editing the filter.
