using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using DotLiquid;
using System.Collections.Generic;
using System.Security;

namespace GeoPhoto.Layouts.GeoPhoto
{
    public class WayPoint {
        public WayPoint(string Title, string Description, double Latitude, double Longitude)
        {
            this.Title = SecurityElement.Escape(Title);
            this.Description = SecurityElement.Escape(Description);
            this.Latitude = Latitude;
            this.Longitude = Longitude;
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class WayPointDrop : Drop
    {
        private readonly WayPoint _waypoint;

        public string Title { get { return _waypoint.Title; } }
        public string Description { get { return _waypoint.Description; } }
        public double Latitude { get { return _waypoint.Latitude; } }
        public double Longitude { get { return _waypoint.Longitude; } }

        public WayPointDrop(WayPoint waypoint) { _waypoint = waypoint; }
    }

    public partial class ExportGPXAction : LayoutsPageBase
    {
        private string gpxTemplate = @"
        <?xml version='1.0' encoding='UTF-8' standalone='no' ?>
        <gpx xmlns='http://www.topografix.com/GPX/1/1' creator='NETSAVANTS' version='1.1' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' 
             xsi:schemaLocation='http://www.topografix.com/GPX/1/1 http://www.topografix.com/GPX/1/1/gpx.xsd'>
            <metadata>
                <bounds maxlat='{{MaxLat}}' maxlon='{{MaxLon}}' minlat='{{MinLat}}' minlon='{{MinLon}}'/>
                <extensions>
                    <time>{{CurrentTime}}</time>
                </extensions>
            </metadata>
            {% for point in Waypoints %}
            <wpt lat='{{point.Latitude}}' lon='{{point.Longitude}}'>
                <time>{{CurrentTime}}</time>
                <name>{{point.Title}}</name>
                <desc>{{point.Description}}</desc>
                <sym>Waypoint</sym>
                <extensions>
                <label>
                    <label_text>{{point.Title}}</label_text>
                </label>
                </extensions>
            </wpt>
            {% endfor %}
        </gpx>
        ";


        protected void Page_Load(object sender, EventArgs e)
        {
            Guid listId;
            Guid viewId;
            try
            {
                listId = new Guid(Request.QueryString["List"]);
                try
                {
                    viewId = new Guid(Request.QueryString["View"]);
                }
                catch (Exception)
                {
                  lblMessage.Text = "Could not find the 'View' parameter in the Url.";
                  return;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
                return;
            }

            try
            {
                string gpx = CreateGPX(listId, viewId);
                Response.Clear();
                Response.ContentType = "application/gpx+xml";
                Response.AddHeader("content-disposition", "attachment; filename=GarminWaypoints.gpx");
                Response.AddHeader("content-length", gpx.Length.ToString());
                Response.Write(gpx);
                Response.End();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "An error occurred during the creation of the GPX file.<br><br>Exception:<br><br>" + ex.Message + "<br>" + ex.StackTrace;
            }

        }

        string CreateGPX(Guid listId, Guid viewId)
        {
            SPList list = Web.Lists[listId];
            SPListItemCollection itemsInView = list.GetItems(list.GetView(viewId));
            List<WayPointDrop> waypoints = new List<WayPointDrop>();
            
            //Get the Min/Max Lat/Long bounds
            double MaxLat = 0.0, MaxLon = 0.0, MinLat = 360.0, MinLon = 360.0;
            foreach (SPListItem item in itemsInView)
            {
                double lat = (double)item["Latitude"];
                double lon = (double)item["Longitude"];

                if (MaxLat < lat) MaxLat = lat;
                if (MaxLon < lon) MaxLon = lon;
                if (MinLat > lat) MinLat = lat;
                if (MinLon > lon) MinLon = lon;
                waypoints.Add(new WayPointDrop(new WayPoint((string)item["Title"], (string)item["Description"], lat, lon))); 
            }

            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("CurrentTime", DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"));
            values.Add("MaxLat", MaxLat.ToString());
            values.Add("MaxLon", MaxLon.ToString());
            values.Add("MinLon", MinLon.ToString());
            values.Add("MinLat", MinLat.ToString());
            values.Add("WayPoints", waypoints);

            Template tm = Template.Parse(gpxTemplate);
            return tm.Render(Hash.FromDictionary(values));
        }
    }
}
