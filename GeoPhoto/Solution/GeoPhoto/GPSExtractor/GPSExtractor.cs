using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using System.Drawing;

namespace GeoPhoto.GPSExtractor
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class GPSExtractor : SPItemEventReceiver
    {
        /// <summary>
        /// An item was added.
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            base.ItemAdded(properties);
            SPListItem item = properties.ListItem;
            Bitmap bmp = new Bitmap(item.File.OpenBinaryStream());

            //Pass the bitmap reference to the Exif processor
            ExifWorks exif = new ExifWorks(ref bmp);
            item["Latitude"] = exif.GPSLatitude;
            item["Longitude"] = exif.GPSLongitude;
            item.SystemUpdate(false);
            bmp.Dispose();
            exif.Dispose();
        }


    }
}