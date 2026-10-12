using System;
using System.Linq;
using System.Collections.Generic;
using Novell.Directory.Ldap;
using System.Globalization;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;

namespace FrameWork.Platform
{
    public enum PasswordModifyType
    {
        Reset,
        Change
    }
    public class LDAP
    {
        //---------------------------------------
        //			Properties
        //---------------------------------------
        private string _usrName = "";
        private string _usrDN = "";
        private string _usrMail = "";
        private string[] _usrGroup;
        private LdapEntry _LdapEntry = null;
        private Dictionary<string, string> _AllAttributes = new Dictionary<string, string>();
        protected readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //---------------------------------------
        //			Constructors
        //---------------------------------------
        public LDAP()
        {
            //* UseLdapCheck 不為1 則不用連Ldap.固定返回成功
            if (LDAPInfo.UseLdapCheck != "1")
            {
                _usrName = "";
                _usrDN = "";
                _usrMail = "";
                _usrGroup = new string[1] { "" };
            }
        }

        public string UserName { get { return _usrName; } }

        public string UserDN { get { return _usrDN; } }

        public string UserMail { get { return _usrMail; } }

        public string[] UserGroup { get { return _usrGroup; } }

        public Dictionary<string, string> AllAttributes { get { return _AllAttributes; } }


        //---------------------------------------
        //			Public Method
        //---------------------------------------
        // authenticate user password
        /// <summary>
        /// 認證使用者LDAP帳號/密碼
        /// </summary>
        /// <param name="as_UserID">使用者登入帳號</param>
        /// <param name="as_UserPWD">使用者登入密碼</param>
        /// <returns></returns>
        public bool AuthenticateUser(string as_UserID, string as_UserPWD)
        {
            //* UseLdapCheck 不為1 則不用連Ldap.固定返回成功
            if (LDAPInfo.UseLdapCheck != "1")
                return true;

            #region 實際Ldap檢核
            try
            {
                bool _valid = false;
                string[] userDNInfo = new string[2] { "", "(samAccountName=" + as_UserID + ")" };//{userDN,searchFilter)
                LdapConnection conn = new LdapConnection();
                try
                {
                    //Step1:get LDAP connection
                    conn.Connect(LDAPInfo.IP, LDAPInfo.Port);

                    //Step2:verify User ID and User password
                    conn.Bind(LDAPInfo.ServiceDN + @"\" + as_UserID, as_UserPWD);

                    //Step3:search user DN
                    LdapSearchResults lsc = conn.Search(LDAPInfo.UserBaseDN, LdapConnection.SCOPE_SUB, userDNInfo[1], null, false);
                    LdapEntry userLdapEntry = null;
                    while (lsc.hasMore())
                    {
                        //LdapEntry nextEntry = null;
                        try
                        {
                            userLdapEntry = lsc.next();
                        }
                        catch (LdapException ex)
                        {
                            throw new Exception(ex.Message);
                        }
                        if (userLdapEntry != null)
                        {
                            _LdapEntry = userLdapEntry;
                            userDNInfo[0] = userLdapEntry.DN.Trim();
                            GetLDAPGroups(userLdapEntry);
                            _AllAttributes = GetAllAttributes();
                            _valid = true;
                            break;
                        }
                    }
                    return _valid;
                    //Windows AD Server 因作業系統安全性考量故不適用
                    //Step4:verify user password
                    //if (!string.IsNullOrEmpty(userDNInfo[0]))
                    //{
                    //    LdapAttribute attr = new LdapAttribute("userPassword", as_UserPWD);
                    //    _valid = conn.Compare(userDNInfo[0], attr);
                    //}

                    //if (_valid)
                    //    GetLDAPGroups(userLdapEntry);
                    //return _valid;
                }
                catch (Exception ex)
                {
                    if (log.IsErrorEnabled) log.Error("AuthenticateUser() Error", ex);
                    throw new Exception("LDAP connect failure!! ");
                }
                finally
                {
                    //Step6:close LDAP connection 
                    conn.Disconnect();
                }
            }
            catch { return false; }
            #endregion
        }

        /// <summary>
        /// 取得使用者在CSFS角色清單
        /// </summary>
        /// <param name="entry"></param>
        private void GetLDAPGroups(LdapEntry entry)
        {
            //* UseLdapCheck 不為1 則不用連Ldap.固定返回成功
            if (LDAPInfo.UseLdapCheck != "1")
                return;

            #region
            LdapAttributeSet attributeSet = entry.getAttributeSet();
            int c = 0;
            string[] tmpUsrGroup;
            _usrGroup = null;
            List<string> tGpList = new List<string>();
            foreach (LdapAttribute attribute in attributeSet)
            {
                switch (attribute.Name)
                {
                    case "fullName":
                    case "name":
                        _usrName = attribute.StringValueArray[0];
                        c++;
                        break;
                    case "mail":
                        _usrMail = attribute.StringValueArray[0];
                        c++;
                        break;
                    case "groupMembership":
                    case "department":
                        tmpUsrGroup = attribute.StringValueArray;//ex:cn=CSAPP01,ou=APPTL,ou=APPs,o=CTCB 
                        foreach (string roleDN in tmpUsrGroup)
                        {
                            tGpList.Add(roleDN);
                        }
                        if (tGpList.Any())
                            _usrGroup = tGpList.ToArray();
                        else
                            _usrGroup = new string[1] { "" };
                        c++;
                        break;
                    default:
                        break;
                }
                if (c == 3) break;//已取得fullName與groupMembership,直接跳離此while loop
            }
        }

        public string GetAttribute(string AttributeName)
        {
            string AttributeVal = "";
            //* UseLdapCheck 不為1 則不用連Ldap.固定返回成功
            if (LDAPInfo.UseLdapCheck != "1")
                return AttributeVal;
            if (_LdapEntry == null)
                return AttributeVal;
            if (_AllAttributes.ContainsKey(AttributeName))
            {
                AttributeVal = _AllAttributes[AttributeName];
            }
            return AttributeVal;
        }
        private Dictionary<string, string> GetAllAttributes()
        {
            Dictionary<string, string> AllAttributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            //* UseLdapCheck 不為1 則不用連Ldap.固定返回成功
            if (LDAPInfo.UseLdapCheck != "1")
                return AllAttributes;
            if (_LdapEntry == null)
                return AllAttributes;
            LdapAttributeSet attributeSet = _LdapEntry.getAttributeSet();
            foreach (LdapAttribute attribute in attributeSet)
            {
                long iVal;
                if (attribute.StringValueArray[0].Length == 18 && long.TryParse(attribute.StringValueArray[0], out iVal))
                {
                    var dt = new DateTime(1601, 01, 01, 0, 0, 0, DateTimeKind.Utc).AddTicks(Convert.ToInt64(attribute.StringValueArray[0]));
                    AllAttributes.Add(attribute.Name, dt.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"));
                }
                else if (attribute.StringValueArray[0].Length == 17 && attribute.StringValueArray[0].LastIndexOf(".0Z") != -1)
                {
                    string format = "yyyyMMddHHmmss.0Z";
                    var dt = DateTime.ParseExact(attribute.StringValueArray[0], format, CultureInfo.InvariantCulture);
                    AllAttributes.Add(attribute.Name, dt.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"));
                }
                else
                {
                    AllAttributes.Add(attribute.Name, attribute.StringValueArray[0]);
                }
            }
            return AllAttributes.OrderBy(dic => dic.Key).ToDictionary(dic => dic.Key, dic => dic.Value, StringComparer.OrdinalIgnoreCase);
        }
        #endregion

        public static String GetRoleLDAPId(string UserGroupInfo)
        {
            string sRoleLDAPId = "";
            if (UserGroupInfo.Equals(""))
            {
                sRoleLDAPId = "";
            }
            else
            {
                if (UserGroupInfo.ToUpper().Contains("CN="))
                {
                    int begIdx = UserGroupInfo.IndexOf("CN=", StringComparison.CurrentCultureIgnoreCase);
                    int endIdx = UserGroupInfo.IndexOf(",OU=", StringComparison.CurrentCultureIgnoreCase);
                    if (endIdx > begIdx)
                    {
                        sRoleLDAPId = UserGroupInfo.Substring(begIdx, endIdx);
                        sRoleLDAPId = sRoleLDAPId.Replace("CN=", "").Replace("cn=", "");
                    }
                    else
                    {
                        sRoleLDAPId = "";
                    }
                }
                else
                {
                    sRoleLDAPId = "";
                }
            }
            return sRoleLDAPId;
        }
        /// <summary>
        /// 加入一個AD User
        /// </summary>
        /// <param name="as_AdminID">連結AD server的帳號</param>
        /// <param name="as_AdminPWD">連結AD server的密碼</param>
        /// <param name="AccountBaseDN">使用者帳號所在的目錄</param>
        /// <param name="UserAccount">新增使用者的帳號</param>
        /// <param name="DisplayName">新增使用者的顯示名稱</param>
        /// <param name="ADGroups">新增使用者的所屬AD群組</param>
        /// <param name="Department">使用者部門</param>
        /// <param name="Email">使用者mail</param>
        /// <param name="AccountExpire">帳號到期日</param>
        /// <returns></returns>
        public bool AddADUser(string as_AdminID, string as_AdminPWD, string AccountBaseDN, string UserAccount, string DisplayName,string[] ADGroups, string Department, string Email, DateTime? AccountExpire = null)
        {
            bool _valid = false;
            LdapConnection conn = new LdapConnection();
            try
            {
                //Step1:get LDAP connection
                conn.Connect(LDAPInfo.IP, LDAPInfo.Port);

                //Step2:verify User ID and User password
                conn.Bind(LDAPInfo.ServiceDN + @"\" + as_AdminID, as_AdminPWD);

                //Step3:check Account no exist, Creates the List attributes of the entry and add them to attribute set                
                LdapSearchResults lsc = conn.Search(LDAPInfo.UserBaseDN, LdapConnection.SCOPE_SUB, "(samAccountName=" + UserAccount + ")", null, false);
                if (!lsc.hasMore())
                {
                    string dn = "cn=" + DisplayName + "," + AccountBaseDN;
                    LdapAttributeSet attributeSet = new LdapAttributeSet();
                    attributeSet.Add(new LdapAttribute("objectclass", "user"));
                    attributeSet.Add(new LdapAttribute("userPrincipalName", UserAccount + "@" + LDAPInfo.ServiceDN + ".local"));
                    attributeSet.Add(new LdapAttribute("samAccountName", UserAccount));
                    attributeSet.Add(new LdapAttribute("cn", new string[] { DisplayName }));
                    attributeSet.Add(new LdapAttribute("displayName", DisplayName));
                    attributeSet.Add(new LdapAttribute("name", DisplayName));
                    attributeSet.Add(new LdapAttribute("department", Department));
                    attributeSet.Add(new LdapAttribute("mail", Email));
                    attributeSet.Add(new LdapAttribute("description", Email));
                    DateTime TargetAccountExpire = AccountExpire ?? DateTime.MinValue;
                    if (AccountExpire != null)
                    {
                        attributeSet.Add(new LdapAttribute("accountExpires", (TargetAccountExpire.ToLocalTime().Ticks - new DateTime(1601, 01, 01, 0, 0, 0, DateTimeKind.Local).Ticks).ToString()));
                    }
                    LdapEntry newEntry = new LdapEntry(dn, attributeSet);
                    conn.Add(newEntry);
                    using (var context = new PrincipalContext(ContextType.Domain, LDAPInfo.ServiceDN, LDAPInfo.UserBaseDN, as_AdminID, as_AdminPWD))
                    {
                        using (var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, UserAccount))
                        {
                            if (user != null)
                            {
                                SetPassword(as_AdminID, as_AdminPWD, UserAccount, PasswordModifyType.Reset);
                                AddUserToGroup(as_AdminID, as_AdminPWD, UserAccount, ADGroups);
                                user.Enabled = true;                                
                                user.ExpirePasswordNow();
                                user.Save();
                                _valid = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled) log.Error("AddADUser() Error", ex);
                throw new Exception(ex.StackTrace);
            }
            finally
            {
                conn.Disconnect();
            }
            return _valid;
        }
        /// <summary>
        /// 刪除一個AD User
        /// </summary>
        /// <param name="as_AdminID">連結AD server的帳號</param>
        /// <param name="as_AdminPWD">連結AD server的密碼</param>
        /// <param name="UserAccount">刪除的AD User帳號</param>
        /// <returns></returns>
        public bool DeleteADUser(string as_AdminID, string as_AdminPWD, string UserAccount)
        {
            bool _valid = false;
            string[] userDNInfo = new string[2] { "", "(samAccountName=" + UserAccount + ")" };//{userDN,searchFilter)
            LdapConnection conn = new LdapConnection();
            try
            {
                //Step1:get LDAP connection
                conn.Connect(LDAPInfo.IP, LDAPInfo.Port);

                //Step2:verify User ID and User password
                conn.Bind(LDAPInfo.ServiceDN + @"\" + as_AdminID, as_AdminPWD);
                //Step3:search user DN
                LdapSearchResults lsc = conn.Search(LDAPInfo.UserBaseDN, LdapConnection.SCOPE_SUB, userDNInfo[1], null, false);
                LdapEntry userLdapEntry = null;
                while (lsc.hasMore())
                {
                    userLdapEntry = lsc.next();
                    if (userLdapEntry != null)
                    {
                        conn.Delete(userLdapEntry.DN);
                        _valid = true;
                    }
                }
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled) log.Error("DeleteADUser() Error", ex);
                throw new Exception(ex.StackTrace);
            }
            finally
            {
                conn.Disconnect();
            }
            return _valid;
        }
        /// <summary>
        /// 停用一個AD User
        /// </summary>
        /// <param name="as_AdminID">連結AD server的帳號</param>
        /// <param name="as_AdminPWD">連結AD server的密碼</param>
        /// <param name="UserAccount">停用的AD User帳號</param>
        /// <returns></returns>
        public bool DisableADUser(string as_AdminID, string as_AdminPWD, string UserAccount)
        {
            bool _valid = false;
            LdapConnection conn = new LdapConnection();
            try
            {
                //Step1:get LDAP connection
                conn.Connect(LDAPInfo.IP, LDAPInfo.Port);

                //Step2:verify User ID and User password
                conn.Bind(LDAPInfo.ServiceDN + @"\" + as_AdminID, as_AdminPWD);
                using (var context = new PrincipalContext(ContextType.Domain, LDAPInfo.ServiceDN, as_AdminID, as_AdminPWD))
                {
                    using (var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, UserAccount))
                    {
                        if (user != null)
                        {
                            user.Enabled = false;
                            user.Save();
                            _valid = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled) log.Error("DiasbleADUser() Error", ex);
                throw new Exception(ex.StackTrace);
            }
            finally
            {
                conn.Disconnect();
            }
            return _valid;
        }
        /// <summary>
        /// 變更AD User密碼
        /// </summary>
        /// <param name="as_AdminID">連結AD server的帳號</param>
        /// <param name="as_AdminPWD">連結AD server的密碼</param>
        /// <param name="UserAccount">使用者帳號</param>
        /// <param name="OldPassword">舊密碼</param>
        /// <param name="NewPassword">新密碼</param>
        /// <returns></returns>
        public bool ChangeADUserPassword(string as_AdminID, string as_AdminPWD, string UserAccount, string OldPassword, string NewPassword)
        {
            bool _valid = false;
            LdapConnection conn = new LdapConnection();
            try
            {
                conn.Connect(LDAPInfo.IP, LDAPInfo.Port);
                conn.Bind(LDAPInfo.ServiceDN + @"\" + as_AdminID, as_AdminPWD);
                using (var context = new PrincipalContext(ContextType.Domain, LDAPInfo.ServiceDN, as_AdminID, as_AdminPWD))
                {
                    using (var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, UserAccount))
                    {
                        if (user != null)
                        {
                            SetPassword(as_AdminID, as_AdminPWD, UserAccount, PasswordModifyType.Change, OldPassword, NewPassword);
                            _valid = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled) log.Error("ChangeADUserPassword() Error", ex);
                throw new Exception(ex.StackTrace);
            }
            finally
            {
                conn.Disconnect();
            }
            return _valid;
        }
        /// <summary>
        /// 重設AD User密碼(預設密碼寫死為1qaz!QAZ)
        /// </summary>
        /// <param name="as_AdminID">連結AD server的帳號</param>
        /// <param name="as_AdminPWD">連結AD server的密碼</param>
        /// <param name="UserAccount">使用者帳號</param>
        /// <returns></returns>
        public bool ResetADUserPassword(string as_AdminID, string as_AdminPWD, string UserAccount)
        {
            bool _valid = false;
            LdapConnection conn = new LdapConnection();
            try
            {
                conn.Connect(LDAPInfo.IP, LDAPInfo.Port);
                conn.Bind(LDAPInfo.ServiceDN + @"\" + as_AdminID, as_AdminPWD);
                using (var context = new PrincipalContext(ContextType.Domain, LDAPInfo.ServiceDN, as_AdminID, as_AdminPWD))
                {
                    using (var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, UserAccount))
                    {
                        if (user != null)
                        {
                            SetPassword(as_AdminID, as_AdminPWD, UserAccount, PasswordModifyType.Reset);
                            user.ExpirePasswordNow();
                            user.Save();
                            _valid = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled) log.Error("ResetADUserPassword() Error", ex);
                throw new Exception(ex.StackTrace);
            }
            finally
            {
                conn.Disconnect();
            }
            return _valid;
        }
        /// <summary>
        /// 設定AD user密碼(分為重設和變更兩種形式)
        /// </summary>
        /// <param name="as_AdminID"></param>
        /// <param name="as_AdminPWD"></param>
        /// <param name="UserAccount"></param>
        /// <param name="ModifyType"></param>
        /// <param name="OldPassword"></param>
        /// <param name="NewPassword"></param>
        private void SetPassword(string as_AdminID, string as_AdminPWD, string UserAccount, PasswordModifyType ModifyType, string OldPassword = null, string NewPassword = null)
        {
            try
            {
                DirectoryEntry userEntry = new DirectoryEntry("WinNT://" + LDAPInfo.ServiceDN + "/" + UserAccount + ",user", as_AdminID, as_AdminPWD);
                if (ModifyType == PasswordModifyType.Reset)
                {
                    userEntry.Invoke("SetPassword", new object[] { "1qaz!QAZ" });
                }
                else if (ModifyType == PasswordModifyType.Change)
                {
                    userEntry.Invoke("ChangePassword", new object[] { OldPassword, NewPassword });
                }
                userEntry.CommitChanges();
                userEntry.Close();
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled) log.Error("ResetADUserPassword() Error", ex);
                throw new Exception(ex.StackTrace);
            }
        }
        /// <summary>
        /// 將AD user加入群組
        /// </summary>
        /// <param name="as_AdminID"></param>
        /// <param name="as_AdminPWD"></param>
        /// <param name="UserAccount"></param>
        /// <param name="GgroupNames"></param>
        private void AddUserToGroup(string as_AdminID, string as_AdminPWD, string UserAccount, string[] GgroupNames)
        {
            try
            {
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, LDAPInfo.ServiceDN, as_AdminID, as_AdminPWD))
                {
                    foreach (string GN in GgroupNames)
                    {
                        GroupPrincipal group = GroupPrincipal.FindByIdentity(context, "CN=" + GN + "," + LDAPInfo.GroupBaseDN);
                        UserPrincipal user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, UserAccount);
                        using (DirectoryEntry groupEntry = group.GetUnderlyingObject() as DirectoryEntry)
                        using (DirectoryEntry userEntry = user.GetUnderlyingObject() as DirectoryEntry)
                        {
                            groupEntry.Invoke("Add", new object[] { userEntry.Path });
                        }
                    }
                }
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        /// <summary>
        /// 取得LDAP連線資訊
        /// </summary>
        public static class LDAPInfo
        {
            public static string UseLdapCheck = (System.Web.Configuration.WebConfigurationManager.AppSettings["UseLdapCheck"] == null ?
                        "" : System.Web.Configuration.WebConfigurationManager.AppSettings["UseLdapCheck"]);
            public static string IP = (System.Web.Configuration.WebConfigurationManager.AppSettings["LDAPServerIP"] == null ?
                        "" : System.Web.Configuration.WebConfigurationManager.AppSettings["LDAPServerIP"]);
            public static int Port = (short)(System.Web.Configuration.WebConfigurationManager.AppSettings["LDAPServerPort"] == null ?
                        0 : Convert.ToInt32(System.Web.Configuration.WebConfigurationManager.AppSettings["LDAPServerPort"]));
            public static string UserBaseDN = (System.Web.Configuration.WebConfigurationManager.AppSettings["LDAPUserBaseDN"] == null ?
                        "" : System.Web.Configuration.WebConfigurationManager.AppSettings["LDAPUserBaseDN"]);
            public static string GroupBaseDN = (System.Web.Configuration.WebConfigurationManager.AppSettings["LDAPGroupBaseDN"] == null ?
                        "" : System.Web.Configuration.WebConfigurationManager.AppSettings["LDAPGroupBaseDN"]);
            public static string ServiceDN = (System.Web.Configuration.WebConfigurationManager.AppSettings["LDAPServiceDN"] == null ?
                        "" : System.Web.Configuration.WebConfigurationManager.AppSettings["LDAPServiceDN"]);
        }
    }
}
