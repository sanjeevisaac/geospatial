using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using DotLiquid;
using Microsoft.SharePoint;
using System.Collections.Generic;

namespace GooglePhotoMap.GoogleMapPart
{
    [ToolboxItemAttribute(false)]
    public partial class GoogleMapPart : WebPart
    {
        [Personalizable(PersonalizationScope.Shared), WebBrowsable(true),
            WebDisplayName("Picture Library Name"),
            WebDescription("Display Name of Picture Library")]
        public string PicLibName { get; set; }

        [Personalizable(PersonalizationScope.Shared), WebBrowsable(true),
            WebDisplayName("Picture Library View Name"),
            WebDescription("Picture Library View that has the Lat/Long columns")]
        public string PicViewName { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                SPList list = SPContext.Current.Web.Lists[PicLibName];
                SPListItemCollection itemsInView = list.GetItems(list.Views[PicViewName]);

                Dictionary<string, string> wpts = new Dictionary<string, string>();
                foreach (SPListItem item in itemsInView)
                {
                    wpts.Add((double)item["Latitude"] + "," + (double)item["Longitude"], (string)item["Title"]);
                }

                waypoints.DataTextField = "Value";
                waypoints.DataValueField = "Key";
                waypoints.DataSource = wpts;
                waypoints.DataBind();
                waypoints.SelectedIndex = 0;
            } catch(Exception){}
            finally {
                Page.RegisterClientScriptBlock("GMap", GoogleMap());
            }
        }

        private string GoogleMap()
        {
            string template = @"
                <link href='http://code.google.com//apis/maps/documentation/javascript/examples/default.css' rel='stylesheet' type='text/css'>
                <meta name='viewport' content='initial-scale=1.0, user-scalable=no' />
                <script src='http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js'></script>
                <script src='http://maps.google.com/maps/api/js?sensor=false'></script>
                 <script>
                      var directionDisplay;
                      var directionsService = new google.maps.DirectionsService();
                      var map;

                      function initialize() {
                        directionsDisplay = new google.maps.DirectionsRenderer();
                        var mapOptions = { zoom: 8, mapTypeId: google.maps.MapTypeId.ROADMAP }
                        map = new google.maps.Map(document.getElementById('map_canvas'), mapOptions);
                        directionsDisplay.setMap(map);

                        var home = new google.maps.LatLng(41.850033, -87.6500523);
                        // Try HTML5 geolocation
                        if(navigator.geolocation) {
                          navigator.geolocation.getCurrentPosition(function(position) {
                             home = new google.maps.LatLng(position.coords.latitude,
                                                             position.coords.longitude);

                            var infowindow = new google.maps.InfoWindow({
                                map: map,
                                position: home,
                                content: 'Location found using HTML5.'
                            });

                            map.setCenter(home);
                          }, function() { handleNoGeolocation(true); });
                        } else {
                          // Browser doesn't support Geolocation
                          handleNoGeolocation(false);
                        }
                      }


                      function handleNoGeolocation(errorFlag) {
                        if (errorFlag) {
                          var content = 'Error: The Geolocation service failed.';
                        } else {
                          var content = 'Error: Your browser doesn\'t support geolocation.';
                        }

                        var options = {
                          map: map,
                          position: new google.maps.LatLng(34, -84),
                          content: content
                        };

                        var infowindow = new google.maps.InfoWindow(options);
                        map.setCenter(options.position);
                      }

                      function calcRoute() {
                        var start = document.getElementById('start').value;
                        var end = document.getElementById('end').value;
                        var waypts = [];
                        var checkboxArray = $('[id$=waypoints]')[0];
                        for (var i = 0; i < checkboxArray.length; i++) {
                          if (checkboxArray.options[i].selected == true) {
                            waypts.push({
                                location:checkboxArray[i].value,
                                stopover:true});
                          }
                        }

                        var request = {
                            origin: start,
                            destination: end,
                            waypoints: waypts,
                            optimizeWaypoints: true,
                            travelMode: google.maps.DirectionsTravelMode.DRIVING
                        };
                        directionsService.route(request, function(response, status) {
                          if (status == google.maps.DirectionsStatus.OK) {
                            directionsDisplay.setDirections(response);
                            var route = response.routes[0];
                            var summaryPanel = document.getElementById('directions_panel');
                            summaryPanel.innerHTML = '';
                            // For each route, display summary information.
                            for (var i = 0; i < route.legs.length; i++) {
                              var routeSegment = i + 1;
                              summaryPanel.innerHTML += '<b>Route Segment: ' + routeSegment + '</b><br>';
                              summaryPanel.innerHTML += route.legs[i].start_address + ' to ';
                              summaryPanel.innerHTML += route.legs[i].end_address + '<br>';
                              summaryPanel.innerHTML += route.legs[i].distance.text + '<br><br>';
                            }
                          }
                        });
                      }
                    </script>
                <script type='text/javascript'>
                    $(document).ready(function() {initialize();});
                </script>
                ";
            return template;
        }
    }
}
