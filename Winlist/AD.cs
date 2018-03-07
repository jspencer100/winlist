using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices;
using System.Collections;

namespace Winlist
{
    class AD
    {

        public enum objectClass
        {
            user, group, computer
        }
        public enum returnType
        {
            distinguishedName, ObjectGUID
        }

        private Domain userDomain = null;


        public Domain GetDomain()
        {

            return System.DirectoryServices.ActiveDirectory.Domain.GetCurrentDomain();
        }

        public string GetDomainString()
        {
            return System.DirectoryServices.ActiveDirectory.Domain.GetCurrentDomain().ToString();

        }

        public AD()
        {
            userDomain = GetDomain();
        }

        public DirectoryEntry GetUser(string userName)
        {
            SearchResultCollection results = null;
            DirectoryEntry entResult = null;
            string strDomainName = "LDAP://" + userDomain;
            DirectoryEntry entry = new DirectoryEntry(strDomainName);
            DirectorySearcher dSearch = new DirectorySearcher(entry);
            dSearch.Filter = "(&(objectClass=user)(l=" + userName + "))";
            results = dSearch.FindAll();

            foreach (SearchResult result in results)
            {
                entResult = result.GetDirectoryEntry();  

            }
            return entResult;
        }

        public string GetObjectDistinguishedName(objectClass objectCls, returnType returnValue, string objectName, string LdapDomain)
        {
            string distinguishedName = string.Empty;
            string connectionPrefix = "LDAP://" + LdapDomain;
            DirectoryEntry entry = new DirectoryEntry(connectionPrefix);
            DirectorySearcher mySearcher = new DirectorySearcher(entry);

            switch (objectCls)
            {
                case objectClass.user:
                    mySearcher.Filter = "(&(objectClass=user)(| (cn = " + objectName + ")(sAMAccountName = " + objectName + ")))";
                    break;
                case objectClass.group:
                    mySearcher.Filter = "(&(objectClass=group)(| (cn = " + objectName + ")(dn = " + objectName + ")))";
                    break;
                case objectClass.computer:
                    mySearcher.Filter = "(&(objectClass=computer)(| (cn = " + objectName + ")(dn = " + objectName + ")))";
                    break;
            }
            SearchResult result = mySearcher.FindOne();

            if (result == null)
            {
                throw new NullReferenceException("unable to locate the distinguishedName for the object " + objectName + " in the " + LdapDomain + " domain");
            }

            DirectoryEntry directoryObject = result.GetDirectoryEntry();
            if (returnValue.Equals(returnType.distinguishedName))
            {
                distinguishedName = "LDAP://" + directoryObject.Properties["distinguishedName"].Value;
            }
            if (returnValue.Equals(returnType.ObjectGUID))
            {
                distinguishedName = directoryObject.Guid.ToString();
            }
            entry.Close();
            entry.Dispose();
            mySearcher.Dispose();
            return distinguishedName;
        }

        public ArrayList AttributeValuesMultiString(string attributeName, string objectDn, ArrayList valuesCollection, bool recursive)
        {
            DirectoryEntry ent = new DirectoryEntry(objectDn);
            PropertyValueCollection ValueCollection = ent.Properties[attributeName];
            IEnumerator en = ValueCollection.GetEnumerator();

            while (en.MoveNext())
            {
                if (en.Current != null)
                {
                    if (!valuesCollection.Contains(en.Current.ToString()))
                    {
                        valuesCollection.Add(en.Current.ToString());
                        if (recursive)
                        {
                            AttributeValuesMultiString(attributeName, "LDAP://" + en.Current.ToString(), valuesCollection, true);
                        }
                    }
                }
            }
            ent.Close();
            ent.Dispose();
            return valuesCollection;
        }


        public ArrayList Groups(string userDn, bool recursive)
        {
            ArrayList groupMemberships = new ArrayList();
            return AttributeValuesMultiString("memberOf", userDn, groupMemberships, recursive);
        }




    }
}
