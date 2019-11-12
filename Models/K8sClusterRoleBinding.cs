﻿using System.Linq;
using Newtonsoft.Json;

namespace ConplementAG.CopsController.Models
{
    public class K8sClusterRoleBinding
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("apiVersion")]
        public string ApiVersion { get; set; }

        [JsonProperty("metadata")]
        public K8sMetadata Metadata { get; set; }

        [JsonProperty("subjects")]
        public K8sSubjectBaseItem[] Subjects { get; set; }

        [JsonProperty("roleRef")]
        public K8sRoleRef RoleRef { get; set; }

        public static K8sClusterRoleBinding CopsNamespaceEditBinding(string namespacename, string[] users, string[] serviceAccounts)
        {
            var subjects = users.ToList()
                    .Select(user => { return new K8sUserSubjectItem(user, "rbac.authorization.k8s.io"); }).ToList<K8sSubjectBaseItem>()
                .Concat(serviceAccounts.ToList()
                    .Select(sa => 
                    { 
                        // TODO error handling
                        return new K8sServiceAccountSubjectItem(sa.Split(".")[0], sa.Split(".")[1]); 
                    }).ToList<K8sSubjectBaseItem>()
                );

            // this is the concrete binding of admin users to the cops namespace edit role (which allows for the edit / delete of own namespaces)
            return new K8sClusterRoleBinding
            {
                Kind = "ClusterRoleBinding",
                ApiVersion = "rbac.authorization.k8s.io/v1",
                Metadata = new K8sMetadata { Name = $"copsnamespace-editor-{namespacename}" },
                RoleRef = new K8sRoleRef("ClusterRole", $"copsnamespace-editor-{namespacename}", "rbac.authorization.k8s.io"),
                Subjects = subjects.ToArray()
            };
        }
    }
}
