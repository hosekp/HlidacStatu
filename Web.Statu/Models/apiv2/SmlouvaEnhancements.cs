/*
 * REST API Hlídače státu
 *
 * REST API Hlídače státu
 *
 * OpenAPI spec version: 2.0.0
 * Contact: podpora@hlidacstatu.cz
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace HlidacStatu.Web.Models.Apiv2
{ 
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class SmlouvaEnhancements : IEquatable<SmlouvaEnhancements>
    { 
        /// <summary>
        /// datum provedení úpravy
        /// </summary>
        /// <value>datum provedení úpravy</value>
        [Required]
        [DataMember(Name="Created")]
        public string Created { get; set; }

        /// <summary>
        /// krátký popis úpravy
        /// </summary>
        /// <value>krátký popis úpravy</value>
        [Required]
        [DataMember(Name="Title")]
        public string Title { get; set; }

        /// <summary>
        /// dlouhý popis úpravy
        /// </summary>
        /// <value>dlouhý popis úpravy</value>
        [Required]
        [DataMember(Name="Description")]
        public  Description { get; set; }

        /// <summary>
        /// Gets or Sets Changed
        /// </summary>
        [Required]
        [DataMember(Name="Changed")]
        public SmlouvaChanged Changed { get; set; }

        /// <summary>
        /// zda je změna veřejná nebo ne
        /// </summary>
        /// <value>zda je změna veřejná nebo ne</value>
        [Required]
        [DataMember(Name="Public")]
        public bool? Public { get; set; }

        /// <summary>
        /// jméno třídy, která provedla změnu
        /// </summary>
        /// <value>jméno třídy, která provedla změnu</value>
        [Required]
        [DataMember(Name="EnhancerType")]
        public string EnhancerType { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SmlouvaEnhancements {\n");
            sb.Append("  Created: ").Append(Created).Append("\n");
            sb.Append("  Title: ").Append(Title).Append("\n");
            sb.Append("  Description: ").Append(Description).Append("\n");
            sb.Append("  Changed: ").Append(Changed).Append("\n");
            sb.Append("  Public: ").Append(Public).Append("\n");
            sb.Append("  EnhancerType: ").Append(EnhancerType).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((SmlouvaEnhancements)obj);
        }

        /// <summary>
        /// Returns true if SmlouvaEnhancements instances are equal
        /// </summary>
        /// <param name="other">Instance of SmlouvaEnhancements to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SmlouvaEnhancements other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    Created == other.Created ||
                    Created != null &&
                    Created.Equals(other.Created)
                ) && 
                (
                    Title == other.Title ||
                    Title != null &&
                    Title.Equals(other.Title)
                ) && 
                (
                    Description == other.Description ||
                    Description != null &&
                    Description.Equals(other.Description)
                ) && 
                (
                    Changed == other.Changed ||
                    Changed != null &&
                    Changed.Equals(other.Changed)
                ) && 
                (
                    Public == other.Public ||
                    Public != null &&
                    Public.Equals(other.Public)
                ) && 
                (
                    EnhancerType == other.EnhancerType ||
                    EnhancerType != null &&
                    EnhancerType.Equals(other.EnhancerType)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)
                    if (Created != null)
                    hashCode = hashCode * 59 + Created.GetHashCode();
                    if (Title != null)
                    hashCode = hashCode * 59 + Title.GetHashCode();
                    if (Description != null)
                    hashCode = hashCode * 59 + Description.GetHashCode();
                    if (Changed != null)
                    hashCode = hashCode * 59 + Changed.GetHashCode();
                    if (Public != null)
                    hashCode = hashCode * 59 + Public.GetHashCode();
                    if (EnhancerType != null)
                    hashCode = hashCode * 59 + EnhancerType.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(SmlouvaEnhancements left, SmlouvaEnhancements right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SmlouvaEnhancements left, SmlouvaEnhancements right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}
