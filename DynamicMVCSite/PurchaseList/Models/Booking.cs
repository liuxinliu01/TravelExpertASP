//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
//Modified by Bachir
//added display names for the columns
namespace PurchaseList.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Booking
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Booking()
        {
            this.BookingDetails = new HashSet<BookingDetail>();
        }
        [Display(Name = "Booking ID:")]
        public int BookingId { get; set; }
        [Display(Name = "Booking Date:")]
        public Nullable<System.DateTime> BookingDate { get; set; }
        [Display(Name = "Booking No:")]
        public string BookingNo { get; set; }
        [Display(Name = "Traveler Count:")]
        public Nullable<double> TravelerCount { get; set; }
        public Nullable<int> CustomerId { get; set; }
        [Display(Name = "Trip Type:")]
        public string TripTypeId { get; set; }
        public Nullable<int> PackageId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BookingDetail> BookingDetails { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Package Package { get; set; }
        public virtual TripType TripType { get; set; }
    }
}
