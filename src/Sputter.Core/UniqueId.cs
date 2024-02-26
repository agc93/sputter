using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Sputter.Core {
    public class UniqueId : IComparable, IComparable<UniqueId>, IEquatable<UniqueId> {
        public static bool MatchOnModelNumber { get; set; } = false;

        [JsonConstructor]
		[Obsolete("Only used for JSON serialization", false)]
		public UniqueId() { }

        [SetsRequiredMembers]
        public UniqueId(string serial, string? modelNumber) {
            SerialNumber = serial;
            ModelNumber = modelNumber;
        }
        public string? ModelNumber { get; set; }
        public required string SerialNumber { get; set; }
        public string? WWN { get; set; }

        public override string ToString() {
            return string.IsNullOrWhiteSpace(ModelNumber)
                ? SerialNumber : $"{ModelNumber}|{SerialNumber}";
        }

        public override bool Equals(object? obj) {
            return Equals(obj as UniqueId);
        }

        public bool Equals(UniqueId? uniqueId) {
            return uniqueId != null && (UniqueId.MatchOnModelNumber && (string.IsNullOrWhiteSpace(ModelNumber) && string.IsNullOrWhiteSpace(uniqueId.ModelNumber))
                ? ToString() == uniqueId.ToString()
                : SerialNumber == uniqueId.SerialNumber);
        }

        public static implicit operator UniqueId(string serial) {
            if (serial.Split('|') is var parts && parts.Length > 1) {
                return new UniqueId(parts[0], parts[1]);
            }
            return new UniqueId(serial, null);
        }

        public static implicit operator string(UniqueId id) {
            return id.ToString();
        }

        public override int GetHashCode() {
            return ToString().GetHashCode();
        }

        int IComparable.CompareTo(object? obj) {
            var other = obj as UniqueId;
            return other == null ? 1 : SerialNumber.CompareTo(other.SerialNumber);
        }
        int IComparable<UniqueId>.CompareTo(UniqueId? other) => SerialNumber.CompareTo(other?.SerialNumber);

        //
        // Summary:
        //     Determines whether two specified ObjectPaths have the same value.
        public static bool operator ==(UniqueId? a, UniqueId? b) {
            //return a is null
            //    ? b is null
            //    : b is null
            //        ? a is null
            //        : a.Equals(b);
            if (a is null) {
                return b is null;
            }
            return a.Equals(b);
        }

        //
        // Summary:
        //     Determines whether two specified ObjectPaths have different values.
        public static bool operator !=(UniqueId? a, UniqueId? b) {
            return !(a == b);
        }
    }
}
