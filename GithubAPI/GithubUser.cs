using System;
using System.Runtime.Serialization;

namespace GithubAPIGet
{
    [DataContract]
    public class GithubUser
    {
        [DataMember(Name="login")]
        public string Username;
        [DataMember(Name="id")]
        public Int64 Id;
        [DataMember(Name="name")]
        public string Name;
        [DataMember(Name="company")]
        public string Company;
        [DataMember(Name="blog")]
        public string Website;
        [DataMember(Name="location")]
        public string Location;
        [DataMember(Name="email")]
        public string Email;
        [DataMember(Name="bio")]
        public string Bio;
        [DataMember(Name="followers")]
        public Int64 Followers;
        [DataMember(Name="following")]
        public Int64 Following;

        public override string ToString()
        {
            return $@"USER: {Username}
USER ID: {Id}
NAME: {Name}
FOLLOWERS: {Followers}
FOLLOWING: {Following}
EMAIL: {Email}
WEBSITE: {Website}
BIO: {Bio}
LOCATION: {Location}
COMPANY: {Company}";
        }
    }
}
