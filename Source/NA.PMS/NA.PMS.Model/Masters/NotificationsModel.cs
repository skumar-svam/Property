using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NA.PMS.Model
{
    public class NotificationsModel
    {
        [DisplayName("Notification Id")]
        public int notificationID { get; set; }

        [Required(ErrorMessage = "Notification name is required")]
        [RegularExpression(@"^[a-zA-Z0-9 ]*$", ErrorMessage = "Notification name should be alphanumeric.")]
        [DisplayName("Notification Name")]
        public string notificationName { get; set; }

        [Required(ErrorMessage = "Scheme is required")]
        [DisplayName("Scheme Id")]
        public int? schemeID { get; set; }
        public string schemeName { get; set; }
        public string createdBy { get; set; }
        public Nullable<System.DateTime> createdDate { get; set; }
        public string modifiedBy { get; set; }
        public Nullable<System.DateTime> modifiedDate { get; set; }

        //[Required(ErrorMessage = "Start date is required")]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> notificationStart { get; set; }

        //[Required(ErrorMessage = "End date is required")]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> notificationEnd { get; set; }

        //[Remote("CheckNotificationPublishDate", "Notifications", AdditionalFields = "schemeID, notificationStart, notificationEnd, publishDate", ErrorMessage = "Publish date should be between scheme start and end date.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> publishDate { get; set; }

        [Required(ErrorMessage = "Booklet cost is required")]
        //[RegularExpression(@"[0-9]+(\.[0-9][0-9]?)?$", ErrorMessage = "Booklet cost must be decimal.")]
        [Range(0.0, 9999.99, ErrorMessage = "Booklet cost must be decimal(4,2).")]
        public decimal? bookletCost { get; set; }
        public bool isActive { get; set; }

        public int mediaTypeId { get; set; }
        public string mediaType { get; set; }
        public int paymentTypeId { get; set; }
        public string paymentType { get; set; }
        public int? oldNotificationID { get; set; }
        public string oldNotificationName { get; set; }

        [Required(ErrorMessage = "Media type is required")]
        public List<int?> schemeMediaType { get; set; }

        [Required(ErrorMessage = "Payment type is required")]
        public List<int?> schemePaymentType { get; set; }

        public List<SchemeModel> schemesList { get; set; }
        public List<MediaModel> mediaList { get; set; }
        public List<PaymentModel> paymentModeList { get; set; }
        public List<int?> selectedPaymentMode { get; set; }
        public List<int?> selectedMediaType { get; set; }
        public IEnumerable<SelectListItem> oldNotificationCollection { get; set; }
        public string EncryptedNotificationId
        {
            get { return CommonHelper.Encode(notificationID.ToString()); }
        }
    }

    public class SchemeNotification
    {
        public int notificationID { get; set; }
        public int schemeID { get; set; }
        public string schemeName { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public decimal bookletCost { get; set; }
        public int oldNotificationId { get; set; }
        public bool isActive { get; set; }
        public string createdBy { get; set; }
        public DateTime createdDate { get; set; }
        public string modifiedBy { get; set; }
        public DateTime lastModified { get; set; }
    }

    public class PaymentModeNotification
    {
        public int refId { get; set; }
        public int notificationId { get; set; }
        public int paymentModeId { get; set; }
        public string paymentModeName { get; set; }
        public bool isActive { get; set; }
        public string createdBy { get; set; }
        public DateTime createdDate { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }
    }

    public class MediaTypeNotification
    {
        public int refId { get; set; }
        public int notificationId { get; set; }
        public int mediaTypeId { get; set; }
        public string mediaTypeName { get; set; }
        public int mediaSubTypeId { get; set; }
        public bool isActive { get; set; }
        public string createdBy { get; set; }
        public DateTime createdDate { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }
    }

    public class SchemeModel
    {
        public int schemeId { get; set; }
        public Nullable<int> schemeTypeId { get; set; }
        public string schemeName { get; set; }
        public Nullable<System.DateTime> startDate { get; set; }
        public Nullable<System.DateTime> endDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string createdBy { get; set; }
        public Nullable<System.DateTime> createdDate { get; set; }
        public string modifiedBy { get; set; }
        public Nullable<System.DateTime> modifiedDate { get; set; }
    }

    public class PaymentModel
    {
        public int paymentModeId { get; set; }
        public string paymentModeName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string createdBy { get; set; }
        public Nullable<System.DateTime> createdDate { get; set; }
        public string modifiedBy { get; set; }
        public Nullable<System.DateTime> modifiedDate { get; set; }
    }

    public class MediaModel
    {
        public int mediaTypeId { get; set; }
        public string mediaType { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string createdBy { get; set; }
        public Nullable<System.DateTime> createdDate { get; set; }
        public string modifiedBy { get; set; }
        public Nullable<System.DateTime> modifiedDate { get; set; }
    }
}
