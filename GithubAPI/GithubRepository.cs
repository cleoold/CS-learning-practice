using System;
using System.Runtime.Serialization;

namespace GithubAPIGet
{
    [DataContract]
    public class GithubRepository
    {
        [DataMember(Name="full_name")]
        public string Name;
        [DataMember(Name="html_url")]
        public string Url;
        [DataMember(Name="description")]
        public string Description;
        [DataMember(Name="fork")]
        public bool IsFork;
        [DataMember(Name="created_at")]
        public string CreatedAt;
        [DataMember(Name="homepage")]
        public string Homepage;
        [DataMember(Name="stargazers_count")]
        public Int64 Stars;
        [DataMember(Name="watchers_count")]
        public Int64 Watchers;
        [DataMember(Name="forks_count")]
        public Int64 Forks;
        [DataMember(Name="language")]
        public string Language;
        [DataMember(Name="open_issues_count")]
        public Int64 OpenIssues;
        [DataMember(Name="license")]
        public GithubLicense License;

        public override string ToString()
        {
            string created = "";
            if (DateTime.TryParse(CreatedAt, out DateTime dt))
                created = dt.ToString();
            return $@"NAME: {Name} {(IsFork ? "(fork)" : "")}
LANGUAGE: {Language}
LICENSE: {(License != null ? License.Name : "")}
DATE CREATED: {created}
URL: {Url}
DESCRIPTION: {Description}
HOMEPAGE: {Homepage}
STARS: {Stars}
WATCHERS: {Watchers}
FORKS: {Forks}
OPEN ISSUES: {OpenIssues}";
        }
    }

    [DataContract]
    public class GithubLicense
    {
        [DataMember(Name="spdx_id")]
        public string Name;
    }
}
