using System;
using Sitecore;
using Sitecore.StringExtensions;
using ZipInfo.Providers.Mongo;

namespace ZipInfo.UI.Admin
{
    public partial class ZipInfo : System.Web.UI.Page
    {
        protected System.Web.UI.HtmlControls.HtmlForm form1;
        protected System.Web.UI.WebControls.CheckBox ForceCheckbox;
        protected System.Web.UI.WebControls.CheckBox WipeCheckbox;
        protected System.Web.UI.WebControls.Button ReloadButton;
        protected System.Web.UI.WebControls.Label ResultLabel;
        protected System.Web.UI.WebControls.Label Label1;
        protected System.Web.UI.WebControls.TextBox ZipLookupTextbox;
        protected System.Web.UI.WebControls.Button LookupButton;
        protected System.Web.UI.WebControls.Literal LookupResultLiteral;
        protected System.Web.UI.WebControls.Button ClearCacheButton;
        protected System.Web.UI.WebControls.Literal CacheStateLiteral;

        protected void Page_Load(object sender, EventArgs e)
        {
            ResultLabel.Text = String.Empty;
            LookupResultLiteral.Text = String.Empty;
            ZipLookupTextbox.Attributes.Add("onkeypress", "button_click(this,'" + this.LookupButton.ClientID + "')");
            CacheStateLiteral.Text = GetCacheState();
        }

        protected void ReloadButton_Click(object sender, EventArgs e)
        {
            ResultLabel.Text = ZipInfoManager.Reload(ForceCheckbox.Checked, WipeCheckbox.Checked);
        }

        protected void ClearCacheButton_Click(object sender, EventArgs e)
        {
            ZipInfoManager.ClearCache();
            CacheStateLiteral.Text = GetCacheState();
        }

        protected void LookupButton_Click(object sender, EventArgs e)
        {
            LookupResultLiteral.Text = String.Empty;
            int zip = 0;
            if (!int.TryParse(ZipLookupTextbox.Text, out zip))
            {
                LookupResultLiteral.Text = "Invalid zip code";
                return;
            }
            LookupResultLiteral.Text += ZipInfoManager.IsCached(zip) ? "(cached)<br>" : "";
            var info = (ZipCode)ZipInfoManager.Get(zip);
            if (info == null)
            {
                LookupResultLiteral.Text = "Not found";
                return;
            }
            LookupResultLiteral.Text +=
                "Zip:{0}<br/>City:{1}<br/>State:{2}<br/>Time zone offset:{3}<br/>DST:{4}<br/>Latitude:{5}<br/>Longitude:{6}<br/>"
                    .FormatWith(
                        info.Zip, info.City, info.State, info.TimezoneOffset, info.ParticipatesInDst, info.Latitude, info.Longitude);
            CacheStateLiteral.Text = GetCacheState();
        }

        private string GetCacheState()
        {
            return "Size: {0}<br>Items: {1}".FormatWith(StringUtil.GetSizeString(ZipInfoManager.GetCacheSize()), ZipInfoManager.GetCacheCount());
        }
    }
}