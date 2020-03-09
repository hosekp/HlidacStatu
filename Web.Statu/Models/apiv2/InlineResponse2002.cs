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
    public partial class InlineResponse2002 : IEquatable<InlineResponse2002>
    { 
        /// <summary>
        /// Celkový počet výsledků v databázi, které odpovídají vyhledávacímu parametru
        /// </summary>
        /// <value>Celkový počet výsledků v databázi, které odpovídají vyhledávacímu parametru</value>
        [Required]
        [DataMember(Name="celkem")]
        public int? Celkem { get; set; }

        /// <summary>
        /// Pole vrácených záznamů
        /// </summary>
        /// <value>Pole vrácených záznamů</value>
        [Required]
        [DataMember(Name="zaznamy")]
        public List<> Zaznamy { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class InlineResponse2002 {\n");
            sb.Append("  Celkem: ").Append(Celkem).Append("\n");
            sb.Append("  Zaznamy: ").Append(Zaznamy).Append("\n");
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
            return obj.GetType() == GetType() && Equals((InlineResponse2002)obj);
        }

        /// <summary>
        /// Returns true if InlineResponse2002 instances are equal
        /// </summary>
        /// <param name="other">Instance of InlineResponse2002 to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(InlineResponse2002 other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    Celkem == other.Celkem ||
                    Celkem != null &&
                    Celkem.Equals(other.Celkem)
                ) && 
                (
                    Zaznamy == other.Zaznamy ||
                    Zaznamy != null &&
                    Zaznamy.SequenceEqual(other.Zaznamy)
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
                    if (Celkem != null)
                    hashCode = hashCode * 59 + Celkem.GetHashCode();
                    if (Zaznamy != null)
                    hashCode = hashCode * 59 + Zaznamy.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(InlineResponse2002 left, InlineResponse2002 right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(InlineResponse2002 left, InlineResponse2002 right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}
