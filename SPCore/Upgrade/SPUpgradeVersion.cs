using System;
using System.Globalization;

namespace SPCore.Upgrade
{
    public class SPUpgradeVersion : ICloneable, IComparable, IComparable<SPUpgradeVersion>, IEquatable<SPUpgradeVersion>
    {
        public int Major { get; private set; }

        public int Minor { get; private set; }

        public int Build { get; private set; }

        public int Revision { get; private set; }

        public SPUpgradeVersion(int major, int minor)
        {
            if (major < 0)
            {
                throw new ArgumentOutOfRangeException("major", "Argument out of range version");
            }
            if (minor < 0)
            {
                throw new ArgumentOutOfRangeException("minor", "Argument out of range version");
            }

            Major = major;
            Minor = minor;
        }

        public SPUpgradeVersion(int major, int minor, int build, int revision)
        {
            if (major < 0)
            {
                throw new ArgumentOutOfRangeException("major", "Argument out of range version");
            }
            if (minor < 0)
            {
                throw new ArgumentOutOfRangeException("minor", "Argument out of range version");
            }
            if (build < 0)
            {
                throw new ArgumentOutOfRangeException("build", "Argument out of range version");
            }
            if (revision < 0)
            {
                throw new ArgumentOutOfRangeException("revision", "Argument out of range version");
            }

            Major = major;
            Minor = minor;
            Build = build;
            Revision = revision;
        }

        public SPUpgradeVersion(string version)
        {
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }
            string[] array = version.Split(new char[] { '.' });
            int num = array.Length;

            if (num < 2 || num > 4)
            {
                throw new ArgumentException("version");
            }

            this.Major = int.Parse(array[0], CultureInfo.InvariantCulture);

            if (this.Major < 0)
            {
                throw new ArgumentOutOfRangeException("Major", "Argument out of range version");
            }

            this.Minor = int.Parse(array[1], CultureInfo.InvariantCulture);

            if (this.Minor < 0)
            {
                throw new ArgumentOutOfRangeException("Minor", "Argument out of range version");
            }

            if (num <= 2) return;

            this.Build = int.Parse(array[2], CultureInfo.InvariantCulture);

            if (this.Build < 0)
            {
                throw new ArgumentOutOfRangeException("Build", "Argument out of range version");
            }

            if (num <= 3) return;

            this.Revision = int.Parse(array[3], CultureInfo.InvariantCulture);

            if (this.Revision < 0)
            {
                throw new ArgumentOutOfRangeException("Revision", "Argument out of range version");
            }
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}.{3}", Major, Minor, Build, Revision);
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            SPUpgradeVersion version = obj as SPUpgradeVersion;
            return CompareTo(version);
        }

        public int CompareTo(SPUpgradeVersion version)
        {
            return version == null
                ? 1
                : (this.Major != version.Major
                    ? (this.Major > version.Major ? 1 : -1)
                    : (this.Minor != version.Minor
                        ? (this.Minor > version.Minor ? 1 : -1)
                        : (this.Build != version.Build
                            ? (this.Build > version.Build ? 1 : -1)
                            : (this.Revision == version.Revision ? 0 : (this.Revision > version.Revision ? 1 : -1)))));
        }

        public override bool Equals(object obj)
        {
            SPUpgradeVersion version = obj as SPUpgradeVersion;
            return Equals(version);
        }

        public bool Equals(SPUpgradeVersion version)
        {
            return version != null && this.Major == version.Major && this.Minor == version.Minor && this.Build == version.Build && this.Revision == version.Revision;
        }

        public override int GetHashCode()
        {
            int num = 0;
            num |= (this.Major & 15) << 28;
            num |= (this.Minor & 255) << 20;
            num |= (this.Build & 255) << 12;
            return num | (this.Revision & 4095);
        }

        public object Clone()
        {
            return new Version(this.Major, this.Minor, this.Build, this.Revision);
        }

        public static bool operator ==(SPUpgradeVersion v1, SPUpgradeVersion v2)
        {
            if (object.ReferenceEquals(v1, null))
            {
                return object.ReferenceEquals(v2, null);
            }
            return v1.Equals(v2);
        }

        public static bool operator !=(SPUpgradeVersion v1, SPUpgradeVersion v2)
        {
            return !(v1 == v2);
        }

        public static bool operator <(SPUpgradeVersion v1, SPUpgradeVersion v2)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException("v1");
            }
            return v1.CompareTo(v2) < 0;
        }

        public static bool operator <=(SPUpgradeVersion v1, SPUpgradeVersion v2)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException("v1");
            }
            return v1.CompareTo(v2) <= 0;
        }

        public static bool operator >(SPUpgradeVersion v1, SPUpgradeVersion v2)
        {
            return v2 < v1;
        }

        public static bool operator >=(SPUpgradeVersion v1, SPUpgradeVersion v2)
        {
            return v2 <= v1;
        }
    }
}
