/*  This file is part of Chummer5a.
 *
 *  Chummer5a is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Chummer5a is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Chummer5a.  If not, see <http://www.gnu.org/licenses/>.
 *
 *  You can obtain the full source code for Chummer5a at
 *  https://github.com/chummer5a/chummer5a
 */
﻿using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

namespace Chummer
{
    public partial class ContactControl : UserControl
    {
        private readonly Contact _objContact;
        private bool _blnLoading = true;
        private readonly int _intLowHeight = 25;
        private readonly int _intFullHeight = 156;

        // Events.
        public EventHandler ContactDetailChanged { get; set; }
        public EventHandler DeleteContact { get; set; }

        #region Control Events
        public ContactControl(Contact objContact)
        {
            InitializeComponent();

            //We don't actually pay for contacts in play so everyone is free
            //Don't present a useless field
            if (objContact.CharacterObject.Created)
            {
                chkFree.Visible = false;
            }
            LanguageManager.TranslateWinForm(GlobalOptions.Language, this);
            MoveControls();

            _objContact = objContact;

            foreach (ToolStripItem objItem in cmsContact.Items)
            {
                LanguageManager.TranslateToolStripItemsRecursively(objItem, GlobalOptions.Language);
            }
        }

        private void ContactControl_Load(object sender, EventArgs e)
        {
            DoubleBuffered = true;
            Width = cmdDelete.Left + cmdDelete.Width;

            LoadContactList();

            DoDataBindings();

            if (_objContact.EntityType == ContactType.Enemy)
            {
                tipTooltip.SetToolTip(imgLink,
                    !string.IsNullOrEmpty(_objContact.FileName)
                        ? LanguageManager.GetString("Tip_Enemy_OpenLinkedEnemy", GlobalOptions.Language)
                        : LanguageManager.GetString("Tip_Enemy_LinkEnemy", GlobalOptions.Language));

                string strTooltip = LanguageManager.GetString("Tip_Enemy_EditNotes", GlobalOptions.Language);
                if (!string.IsNullOrEmpty(_objContact.Notes))
                    strTooltip += "\n\n" + _objContact.Notes;
                tipTooltip.SetToolTip(imgNotes, strTooltip.WordWrap(100));
            }
            else
            {
                tipTooltip.SetToolTip(imgLink,
                    !string.IsNullOrEmpty(_objContact.FileName)
                        ? LanguageManager.GetString("Tip_Contact_OpenLinkedContact", GlobalOptions.Language)
                        : LanguageManager.GetString("Tip_Contact_LinkContact", GlobalOptions.Language));

                string strTooltip = LanguageManager.GetString("Tip_Contact_EditNotes", GlobalOptions.Language);
                if (!string.IsNullOrEmpty(_objContact.Notes))
                    strTooltip += "\n\n" + _objContact.Notes;
                tipTooltip.SetToolTip(imgNotes, strTooltip.WordWrap(100));
            }

            _blnLoading = false;
        }

        private void nudConnection_ValueChanged(object sender, EventArgs e)
        {
            // Raise the ContactDetailChanged Event when the NumericUpDown's Value changes.
            if (!_blnLoading)
                ContactDetailChanged?.Invoke(this, new TextEventArgs("Connection"));
        }

        private void nudLoyalty_ValueChanged(object sender, EventArgs e)
        {
            // Raise the ContactDetailChanged Event when the NumericUpDown's Value changes.
            // The entire ContactControl is passed as an argument so the handling event can evaluate its contents.
            if (!_blnLoading)
                ContactDetailChanged?.Invoke(this, new TextEventArgs("Loyalty"));
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            // Raise the DeleteContact Event when the user has confirmed their desire to delete the Contact.
            // The entire ContactControl is passed as an argument so the handling event can evaluate its contents.
            DeleteContact?.Invoke(this, e);
        }

        private void chkGroup_CheckedChanged(object sender, EventArgs e)
        {
            if (!_blnLoading)
                ContactDetailChanged?.Invoke(this, new TextEventArgs("Group"));
        }

        
        private void cmdExpand_Click(object sender, EventArgs e)
        {
            Expanded = !Expanded;
        }

        private void cboContactRole_TextChanged(object sender, EventArgs e)
        {
            if (!_blnLoading)
                ContactDetailChanged?.Invoke(this, new TextEventArgs("Role"));
        }

        private void txtContactName_TextChanged(object sender, EventArgs e)
        {
            if (!_blnLoading)
                ContactDetailChanged?.Invoke(this, new TextEventArgs("Name"));
        }

        private void txtContactLocation_TextChanged(object sender, EventArgs e)
        {
            if (!_blnLoading)
                ContactDetailChanged?.Invoke(this, new TextEventArgs("Location"));
        }

        private void cboMetatype_TextChanged(object sender, EventArgs e)
        {
            if (!_blnLoading)
                ContactDetailChanged?.Invoke(this, new TextEventArgs("Metatype"));
        }

        private void cboSex_TextChanged(object sender, EventArgs e)
        {
            if (!_blnLoading)
                ContactDetailChanged?.Invoke(this, new TextEventArgs("Sex"));
        }

        private void cboAge_TextChanged(object sender, EventArgs e)
        {
            if (!_blnLoading)
                ContactDetailChanged?.Invoke(this, new TextEventArgs("Age"));
        }

        private void cboPersonalLife_TextChanged(object sender, EventArgs e)
        {
            if (!_blnLoading)
                ContactDetailChanged?.Invoke(this, e);
        }

        private void cboType_TextChanged(object sender, EventArgs e)
        {
            if (!_blnLoading)
                ContactDetailChanged?.Invoke(this, new TextEventArgs("Type"));
        }

        private void cboPreferredPayment_TextChanged(object sender, EventArgs e)
        {
            if (!_blnLoading)
                ContactDetailChanged?.Invoke(this, new TextEventArgs("PreferredPayment"));
        }

        private void cboHobbiesVice_TextChanged(object sender, EventArgs e)
        {
            if (!_blnLoading)
                ContactDetailChanged?.Invoke(this, new TextEventArgs("HobbiesVice"));
        }

        private void imgLink_Click(object sender, EventArgs e)
        {
            // Determine which options should be shown based on the FileName value.
            if (!string.IsNullOrEmpty(_objContact.FileName))
            {
                tsAttachCharacter.Visible = false;
                tsContactOpen.Visible = true;
                tsRemoveCharacter.Visible = true;
            }
            else
            {
                tsAttachCharacter.Visible = true;
                tsContactOpen.Visible = false;
                tsRemoveCharacter.Visible = false;
            }
            cmsContact.Show(imgLink, imgLink.Left - 490, imgLink.Top);
        }

        private void tsContactOpen_Click(object sender, EventArgs e)
        {
            if (_objContact.LinkedCharacter != null)
            {
                Character objOpenCharacter = Program.MainForm.OpenCharacters.FirstOrDefault(x => x == _objContact.LinkedCharacter);
                Cursor = Cursors.WaitCursor;
                if (objOpenCharacter == null || !Program.MainForm.SwitchToOpenCharacter(objOpenCharacter, true))
                {
                    objOpenCharacter = Program.MainForm.LoadCharacter(_objContact.LinkedCharacter.FileName);
                    Program.MainForm.OpenCharacter(objOpenCharacter);
                }
                Cursor = Cursors.Default;
            }
            else
            {
                bool blnUseRelative = false;

                // Make sure the file still exists before attempting to load it.
                if (!File.Exists(_objContact.FileName))
                {
                    bool blnError = false;
                    // If the file doesn't exist, use the relative path if one is available.
                    if (string.IsNullOrEmpty(_objContact.RelativeFileName))
                        blnError = true;
                    else if (!File.Exists(Path.GetFullPath(_objContact.RelativeFileName)))
                        blnError = true;
                    else
                        blnUseRelative = true;

                    if (blnError)
                    {
                        MessageBox.Show(LanguageManager.GetString("Message_FileNotFound", GlobalOptions.Language).Replace("{0}", _objContact.FileName), LanguageManager.GetString("MessageTitle_FileNotFound", GlobalOptions.Language), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                string strFile = blnUseRelative ? Path.GetFullPath(_objContact.RelativeFileName) : _objContact.FileName;
                System.Diagnostics.Process.Start(strFile);
            }
        }

        private void tsAttachCharacter_Click(object sender, EventArgs e)
        {
            // Prompt the user to select a save file to associate with this Contact.
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Chummer5 Files (*.chum5)|*.chum5|All Files (*.*)|*.*"
            };
            if (!string.IsNullOrEmpty(_objContact.FileName) && File.Exists(_objContact.FileName))
            {
                openFileDialog.InitialDirectory = Path.GetDirectoryName(_objContact.FileName);
                openFileDialog.FileName = Path.GetFileName(_objContact.FileName);
            }

            if (openFileDialog.ShowDialog(this) != DialogResult.OK) return;
            _objContact.FileName = openFileDialog.FileName;
            tipTooltip.SetToolTip(imgLink,
                _objContact.EntityType == ContactType.Enemy
                    ? LanguageManager.GetString("Tip_Enemy_OpenFile", GlobalOptions.Language)
                    : LanguageManager.GetString("Tip_Contact_OpenFile", GlobalOptions.Language));

            // Set the relative path.
            Uri uriApplication = new Uri(@Application.StartupPath);
            Uri uriFile = new Uri(@_objContact.FileName);
            Uri uriRelative = uriApplication.MakeRelativeUri(uriFile);
            _objContact.RelativeFileName = "../" + uriRelative;

            ContactDetailChanged?.Invoke(this, new TextEventArgs("File"));
        }

        private void tsRemoveCharacter_Click(object sender, EventArgs e)
        {
            // Remove the file association from the Contact.
            if (MessageBox.Show(LanguageManager.GetString("Message_RemoveCharacterAssociation", GlobalOptions.Language), LanguageManager.GetString("MessageTitle_RemoveCharacterAssociation", GlobalOptions.Language), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _objContact.FileName = string.Empty;
                _objContact.RelativeFileName = string.Empty;
                tipTooltip.SetToolTip(imgLink,
                    _objContact.EntityType == ContactType.Enemy
                        ? LanguageManager.GetString("Tip_Enemy_LinkFile", GlobalOptions.Language)
                        : LanguageManager.GetString("Tip_Contact_LinkFile", GlobalOptions.Language));
                ContactDetailChanged?.Invoke(this, new TextEventArgs("File"));
            }
        }

        private void imgNotes_Click(object sender, EventArgs e)
        {
            frmNotes frmContactNotes = new frmNotes
            {
                Notes = _objContact.Notes
            };
            frmContactNotes.ShowDialog(this);

            if (frmContactNotes.DialogResult == DialogResult.OK)
                _objContact.Notes = frmContactNotes.Notes;

            string strTooltip = LanguageManager.GetString(_objContact.EntityType == ContactType.Enemy ? "Tip_Enemy_EditNotes" : "Tip_Contact_EditNotes", GlobalOptions.Language);
            if (!string.IsNullOrEmpty(_objContact.Notes))
                strTooltip += "\n\n" + _objContact.Notes;
            tipTooltip.SetToolTip(imgNotes, strTooltip.WordWrap(100));
        }

        private void chkFree_CheckedChanged(object sender, EventArgs e)
        {
            if (!_blnLoading)
                ContactDetailChanged?.Invoke(this, new TextEventArgs("Free"));
        }

        private void chkBlackmail_CheckedChanged(object sender, EventArgs e)
        {
            if (!_blnLoading)
                ContactDetailChanged?.Invoke(this, new TextEventArgs("Blackmail"));
        }

        private void chkFamily_CheckedChanged(object sender, EventArgs e)
        {
            if (!_blnLoading)
                ContactDetailChanged?.Invoke(this, new TextEventArgs("Family"));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Contact object this is linked to.
        /// </summary>
        public Contact ContactObject
        {
            get
            {
                return _objContact;
            }
        }

        public bool Expanded
        {
            get => Height > _intLowHeight;
            set
            {
                if (value)
                {
                    Height = _intFullHeight;
                    cmdExpand.Image = Properties.Resources.Expand;
                }
                else
                {
                    Height = _intLowHeight;
                    cmdExpand.Image = Properties.Resources.Collapse;
                }
            }
        }
        #endregion

        #region Methods
        private void LoadContactList()
        {
            if (_objContact.EntityType == ContactType.Enemy)
            {
                string strContactRole = _objContact.DisplayRole;
                if (!string.IsNullOrEmpty(strContactRole))
                    cboContactRole.Text = strContactRole;
                return;
            }

            if (_objContact.ReadOnly)
            {
                chkFree.Enabled = chkGroup.Enabled =
                nudConnection.Enabled = nudLoyalty.Enabled = false;

                cmdDelete.Visible = false;
            }


            // Read the list of Categories from the XML file.
            List<ListItem> lstCategories = new List<ListItem>
            {
                ListItem.Blank
            };
            List<ListItem> lstMetatypes = new List<ListItem>
            {
                ListItem.Blank
            };
            List<ListItem> lstSexes = new List<ListItem>
            {
                ListItem.Blank
            };
            List<ListItem> lstAges = new List<ListItem>
            {
                ListItem.Blank
            };
            List<ListItem> lstPersonalLives = new List<ListItem>
            {
                ListItem.Blank
            };
            List<ListItem> lstTypes = new List<ListItem>
            {
                ListItem.Blank
            };
            List<ListItem> lstPreferredPayments = new List<ListItem>
            {
                ListItem.Blank
            };
            List<ListItem> lstHobbiesVices = new List<ListItem>
            {
                ListItem.Blank
            };

            XmlDocument objXmlDocument = XmlManager.Load("contacts.xml");
            foreach (XmlNode objXmlNode in objXmlDocument.SelectNodes("/chummer/contacts/contact"))
            {
                string strName = objXmlNode.InnerText;
                lstCategories.Add(new ListItem(strName, objXmlNode.Attributes?["translate"]?.InnerText ?? strName));
            }
            foreach (XmlNode objXmlNode in objXmlDocument.SelectNodes("/chummer/sexes/sex"))
            {
                string strName = objXmlNode.InnerText;
                lstSexes.Add(new ListItem(strName, objXmlNode.Attributes?["translate"]?.InnerText ?? strName));
            }
            foreach (XmlNode objXmlNode in objXmlDocument.SelectNodes("/chummer/ages/age"))
            {
                string strName = objXmlNode.InnerText;
                lstAges.Add(new ListItem(strName, objXmlNode.Attributes?["translate"]?.InnerText ?? strName));
            }
            foreach (XmlNode objXmlNode in objXmlDocument.SelectNodes("/chummer/personallives/personallife"))
            {
                string strName = objXmlNode.InnerText;
                lstPersonalLives.Add(new ListItem(strName, objXmlNode.Attributes?["translate"]?.InnerText ?? strName));
            }
            foreach (XmlNode objXmlNode in objXmlDocument.SelectNodes("/chummer/types/type"))
            {
                string strName = objXmlNode.InnerText;
                lstTypes.Add(new ListItem(strName, objXmlNode.Attributes?["translate"]?.InnerText ?? strName));
            }
            foreach (XmlNode objXmlNode in objXmlDocument.SelectNodes("/chummer/preferredpayments/preferredpayment"))
            {
                string strName = objXmlNode.InnerText;
                lstPreferredPayments.Add(new ListItem(strName, objXmlNode.Attributes?["translate"]?.InnerText ?? strName));
            }
            foreach (XmlNode objXmlNode in objXmlDocument.SelectNodes("/chummer/hobbiesvices/hobbyvice"))
            {
                string strName = objXmlNode.InnerText;
                lstHobbiesVices.Add(new ListItem(strName, objXmlNode.Attributes?["translate"]?.InnerText ?? strName));
            }
            foreach (XmlNode xmlMetatypeNode in XmlManager.Load("metatypes.xml")?.SelectNodes("/chummer/metatypes/metatype"))
            {
                string strName = xmlMetatypeNode["name"].InnerText;
                string strMetatypeDisplay = xmlMetatypeNode["translate"]?.InnerText ?? strName;
                lstMetatypes.Add(new ListItem(strName, strMetatypeDisplay));
                foreach (XmlNode objXmlMetavariantNode in xmlMetatypeNode.SelectNodes("metavariants/metavariant"))
                {
                    string strMetavariantName = objXmlMetavariantNode["name"].InnerText;
                    if (lstMetatypes.All(x => x.Value != strMetavariantName))
                        lstMetatypes.Add(new ListItem(strMetavariantName, strMetatypeDisplay + " (" + (objXmlMetavariantNode["translate"]?.InnerText ?? strMetavariantName) + ')'));
                }
            }
            
            lstCategories.Sort(CompareListItems.CompareNames);
            lstMetatypes.Sort(CompareListItems.CompareNames);
            lstSexes.Sort(CompareListItems.CompareNames);
            lstAges.Sort(CompareListItems.CompareNames);
            lstPersonalLives.Sort(CompareListItems.CompareNames);
            lstTypes.Sort(CompareListItems.CompareNames);
            lstHobbiesVices.Sort(CompareListItems.CompareNames);
            lstPreferredPayments.Sort(CompareListItems.CompareNames);

            cboContactRole.BeginUpdate();
            cboContactRole.ValueMember = "Value";
            cboContactRole.DisplayMember = "Name";
            cboContactRole.DataSource = lstCategories;
            cboContactRole.EndUpdate();

            cboMetatype.BeginUpdate();
            cboMetatype.ValueMember = "Value";
            cboMetatype.DisplayMember = "Name";
            cboMetatype.DataSource = lstMetatypes;
            cboMetatype.EndUpdate();

            cboSex.BeginUpdate();
            cboSex.ValueMember = "Value";
            cboSex.DisplayMember = "Name";
            cboSex.DataSource = lstSexes;
            cboSex.EndUpdate();

            cboAge.BeginUpdate();
            cboAge.ValueMember = "Value";
            cboAge.DisplayMember = "Name";
            cboAge.DataSource = lstAges;
            cboAge.EndUpdate();

            cboPersonalLife.BeginUpdate();
            cboPersonalLife.ValueMember = "Value";
            cboPersonalLife.DisplayMember = "Name";
            cboPersonalLife.DataSource = lstPersonalLives;
            cboPersonalLife.EndUpdate();

            cboType.BeginUpdate();
            cboType.ValueMember = "Value";
            cboType.DisplayMember = "Name";
            cboType.DataSource = lstTypes;
            cboType.EndUpdate();

            cboPreferredPayment.BeginUpdate();
            cboPreferredPayment.ValueMember = "Value";
            cboPreferredPayment.DisplayMember = "Name";
            cboPreferredPayment.DataSource = lstPreferredPayments;
            cboPreferredPayment.EndUpdate();

            cboHobbiesVice.BeginUpdate();
            cboHobbiesVice.ValueMember = "Value";
            cboHobbiesVice.DisplayMember = "Name";
            cboHobbiesVice.DataSource = lstHobbiesVices;
            cboHobbiesVice.EndUpdate();
        }

        private void DoDataBindings()
        {
            chkGroup.DataBindings.Add("Checked", _objContact, nameof(_objContact.IsGroupOrMadeMan), false,
                DataSourceUpdateMode.OnPropertyChanged);
            chkGroup.DataBindings.Add("Enabled", _objContact, nameof(_objContact.NotMadeMan), false,
                DataSourceUpdateMode.OnPropertyChanged);
            chkFree.DataBindings.Add("Checked", _objContact, nameof(_objContact.Free), false,
                DataSourceUpdateMode.OnPropertyChanged);
            chkFamily.DataBindings.Add("Checked", _objContact, nameof(_objContact.Family), false,
                DataSourceUpdateMode.OnPropertyChanged);
            chkFamily.DataBindings.Add("Visible", _objContact, nameof(_objContact.IsNotEnemy), false,
                DataSourceUpdateMode.OnPropertyChanged);
            chkBlackmail.DataBindings.Add("Checked", _objContact, nameof(_objContact.Blackmail), false,
                DataSourceUpdateMode.OnPropertyChanged);
            chkBlackmail.DataBindings.Add("Visible", _objContact, nameof(_objContact.IsNotEnemy), false,
                DataSourceUpdateMode.OnPropertyChanged);
            lblQuickStats.DataBindings.Add("Text", _objContact, nameof(_objContact.QuickText), false,
                DataSourceUpdateMode.OnPropertyChanged);
            nudLoyalty.DataBindings.Add("Value", _objContact, nameof(_objContact.Loyalty), false,
                DataSourceUpdateMode.OnPropertyChanged);
            nudLoyalty.DataBindings.Add("Enabled", _objContact, nameof(_objContact.LoyaltyEnabled), false,
                DataSourceUpdateMode.OnPropertyChanged);
            nudConnection.DataBindings.Add("Value", _objContact, nameof(_objContact.Connection), false,
                DataSourceUpdateMode.OnPropertyChanged);
            nudConnection.DataBindings.Add("Maximum", _objContact, nameof(_objContact.ConnectionMaximum), false,
                DataSourceUpdateMode.OnPropertyChanged);
            txtContactName.DataBindings.Add("Text", _objContact, nameof(_objContact.Name), false,
                DataSourceUpdateMode.OnPropertyChanged);
            txtContactLocation.DataBindings.Add("Text", _objContact, nameof(_objContact.Location), false,
                DataSourceUpdateMode.OnPropertyChanged);
            cboContactRole.DataBindings.Add("Text", _objContact, nameof(_objContact.DisplayRole), false,
                DataSourceUpdateMode.OnPropertyChanged);
            cboMetatype.DataBindings.Add("Text", _objContact, nameof(_objContact.DisplayMetatype), false,
                DataSourceUpdateMode.OnPropertyChanged);
            cboSex.DataBindings.Add("Text", _objContact, nameof(_objContact.DisplaySex), false,
                DataSourceUpdateMode.OnPropertyChanged);
            cboAge.DataBindings.Add("Text", _objContact, nameof(_objContact.DisplayAge), false,
                DataSourceUpdateMode.OnPropertyChanged);
            cboPersonalLife.DataBindings.Add("Text", _objContact, nameof(_objContact.DisplayPersonalLife), false,
                DataSourceUpdateMode.OnPropertyChanged);
            cboType.DataBindings.Add("Text", _objContact, nameof(_objContact.DisplayType), false,
                DataSourceUpdateMode.OnPropertyChanged);
            cboPreferredPayment.DataBindings.Add("Text", _objContact, nameof(_objContact.DisplayPreferredPayment), false,
                DataSourceUpdateMode.OnPropertyChanged);
            cboHobbiesVice.DataBindings.Add("Text", _objContact, nameof(_objContact.DisplayHobbiesVice), false,
                DataSourceUpdateMode.OnPropertyChanged);
            this.DataBindings.Add("BackColor", _objContact, nameof(_objContact.Colour), false,
                DataSourceUpdateMode.OnPropertyChanged);

            // Properties controllable by the character themselves
            txtContactName.DataBindings.Add("Enabled", _objContact, nameof(_objContact.NoLinkedCharacter), false,
                DataSourceUpdateMode.OnPropertyChanged);
            cboMetatype.DataBindings.Add("Enabled", _objContact, nameof(_objContact.NoLinkedCharacter), false,
                DataSourceUpdateMode.OnPropertyChanged);
            cboSex.DataBindings.Add("Enabled", _objContact, nameof(_objContact.NoLinkedCharacter), false,
                DataSourceUpdateMode.OnPropertyChanged);
            cboAge.DataBindings.Add("Enabled", _objContact, nameof(_objContact.NoLinkedCharacter), false,
                DataSourceUpdateMode.OnPropertyChanged);
        }

        private void MoveControls()
        {
            lblConnection.Left = txtContactName.Left;
            nudConnection.Left = lblConnection.Right + 2;
            lblLoyalty.Left = nudConnection.Right + 2;
            nudLoyalty.Left = lblLoyalty.Right + 2;
            imgLink.Left = nudLoyalty.Right + 4; 
            imgNotes.Left = imgLink.Right + 4;
            chkGroup.Left = imgNotes.Right + 4;
            chkFree.Left = chkGroup.Right + 2;
            chkBlackmail.Left = chkFree.Right + 2;
            chkFamily.Left = chkBlackmail.Right + 2;

            lblMetatype.Left = cboMetatype.Left - 7 - lblMetatype.Width;
            lblAge.Left = cboAge.Left - 7 - lblAge.Width;
            lblSex.Left = cboSex.Left - 7 - lblSex.Width;
            lblPersonalLife.Left = cboPersonalLife.Left - 7 - lblPersonalLife.Width;
            lblType.Left = cboType.Left - 7 - lblType.Width;
            lblPreferredPayment.Left = cboPreferredPayment.Left - 7 - lblPreferredPayment.Width;
            lblHobbiesVice.Left = cboHobbiesVice.Left - 7 - lblHobbiesVice.Width;
        }
        #endregion
    }
}
