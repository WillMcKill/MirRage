using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Server.MirEnvir;

namespace Server
{
    public partial class ItemInfoForm : Form
    {
        public string ItemListPath = Path.Combine(Settings.ExportPath, "ItemList.txt");

        public Envir Envir
        {
            get { return SMain.EditEnvir; }
        }
        private List<ItemInfo> _selectedItemInfos;

        public class ComboBoxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        public ItemInfoForm()
        {
            InitializeComponent();

            ITypeComboBox.Items.AddRange(Enum.GetValues(typeof (ItemType)).Cast<object>().ToArray());
            RTypeComboBox.Items.AddRange(Enum.GetValues(typeof (RequiredType)).Cast<object>().ToArray());
            RClassComboBox.Items.AddRange(Enum.GetValues(typeof (RequiredClass)).Cast<object>().ToArray());
            RGenderComboBox.Items.AddRange(Enum.GetValues(typeof (RequiredGender)).Cast<object>().ToArray());
            ISetComboBox.Items.AddRange(Enum.GetValues(typeof(ItemSet)).Cast<object>().ToArray());

            ITypeFilterComboBox.Items.AddRange(Enum.GetValues(typeof(ItemType)).Cast<object>().ToArray());
            ITypeFilterComboBox.Items.Add(new ComboBoxItem { Text = "All" });
            ITypeFilterComboBox.SelectedIndex = ITypeFilterComboBox.Items.Count - 1;

            UpdateInterface();
        }

        public void UpdateInterface(bool refreshList = false)
        {
            if (refreshList)
            {
                ItemInfoListBox.Items.Clear();

                for (int i = 0; i < Envir.ItemInfoList.Count; i++)
                {
                    if (ITypeFilterComboBox.SelectedItem == null ||
                        ITypeFilterComboBox.SelectedIndex == ITypeFilterComboBox.Items.Count - 1 ||
                        Envir.ItemInfoList[i].Type == (ItemType)ITypeFilterComboBox.SelectedItem)
                        ItemInfoListBox.Items.Add(Envir.ItemInfoList[i]);
                }
            }

            _selectedItemInfos = ItemInfoListBox.SelectedItems.Cast<ItemInfo>().ToList();


            if (_selectedItemInfos.Count == 0)
            {
                ItemInfoPanel.Enabled = false;

                ItemIndexTextBox.Text = string.Empty;
                ItemNameTextBox.Text = string.Empty;
                WeightTextBox.Text = string.Empty;
                ImageTextBox.Text = string.Empty;
                DuraTextBox.Text = string.Empty;
                ITypeComboBox.SelectedItem = null;
                ISetComboBox.SelectedItem = null;
                ShapeTextBox.Text = string.Empty;
                SSizeTextBox.Text = string.Empty;
                PriceTextBox.Text = string.Empty;
                RTypeComboBox.SelectedItem = null;
                RAmountTextBox.Text = string.Empty;
                RClassComboBox.SelectedItem = null;
                RGenderComboBox.SelectedItem = null;            
                LightTextBox.Text = string.Empty;         

                MinACTextBox.Text = string.Empty;
                MaxACTextBox.Text = string.Empty;
                MinMACTextBox.Text = string.Empty;
                MaxMACTextBox.Text = string.Empty;
                MinDCTextBox.Text = string.Empty;
                MaxDCTextBox.Text = string.Empty;
                MinMCTextBox.Text = string.Empty;
                MaxMCTextBox.Text = string.Empty;
                MinSCTextBox.Text = string.Empty;
                MaxSCTextBox.Text = string.Empty;
                HPTextBox.Text = string.Empty;
                MPTextBox.Text = string.Empty;
                AccuracyTextBox.Text = string.Empty;
                AgilityTextBox.Text = string.Empty;
                ASpeedTextBox.Text = string.Empty;
                LuckTextBox.Text = string.Empty;
                StartItemCheckBox.Checked = false;

                WWeightTextBox.Text = string.Empty;
                HWeightTextBox.Text = string.Empty;
                BWeightText.Text = string.Empty;
                EffectTextBox.Text = string.Empty;
                return;
            }

            ItemInfo info = _selectedItemInfos[0];

            ItemInfoPanel.Enabled = true;

            ItemIndexTextBox.Text = info.Index.ToString();
            ItemNameTextBox.Text = info.Name;
            WeightTextBox.Text = info.Weight.ToString();
            ImageTextBox.Text = info.Image.ToString();
            DuraTextBox.Text = info.Durability.ToString();
            ITypeComboBox.SelectedItem = info.Type;
            ISetComboBox.SelectedItem = info.Set;
            ShapeTextBox.Text = info.Shape.ToString();
            SSizeTextBox.Text = info.StackSize.ToString();
            PriceTextBox.Text = info.Price.ToString();
            RTypeComboBox.SelectedItem = info.RequiredType;
            RAmountTextBox.Text = info.RequiredAmount.ToString();
            RClassComboBox.SelectedItem = info.RequiredClass;
            RGenderComboBox.SelectedItem = info.RequiredGender;
            LightTextBox.Text = info.Light.ToString();          

            MinACTextBox.Text = info.MinAC.ToString();
            MaxACTextBox.Text = info.MaxAC.ToString();
            MinMACTextBox.Text = info.MinMAC.ToString();
            MaxMACTextBox.Text = info.MaxMAC.ToString();
            MinDCTextBox.Text = info.MinDC.ToString();
            MaxDCTextBox.Text = info.MaxDC.ToString();
            MinMCTextBox.Text = info.MinMC.ToString();
            MaxMCTextBox.Text = info.MaxMC.ToString();
            MinSCTextBox.Text = info.MinSC.ToString();
            MaxSCTextBox.Text = info.MaxSC.ToString();
            HPTextBox.Text = info.HP.ToString();
            MPTextBox.Text = info.MP.ToString();
            AccuracyTextBox.Text = info.Accuracy.ToString();
            AgilityTextBox.Text = info.Agility.ToString();
            ASpeedTextBox.Text = info.AttackSpeed.ToString();
            LuckTextBox.Text = info.Luck.ToString();

            WWeightTextBox.Text = info.WearWeight.ToString();
            HWeightTextBox.Text = info.HandWeight.ToString();
            BWeightText.Text = info.BagWeight.ToString();

            StartItemCheckBox.Checked = info.StartItem;
            EffectTextBox.Text = info.Effect.ToString();

            for (int i = 1; i < _selectedItemInfos.Count; i++)
            {
                info = _selectedItemInfos[i];

                if (ItemIndexTextBox.Text != info.Index.ToString()) ItemIndexTextBox.Text = string.Empty;
                if (ItemNameTextBox.Text != info.Name) ItemNameTextBox.Text = string.Empty;

                if (WeightTextBox.Text != info.Weight.ToString()) WeightTextBox.Text = string.Empty;
                if (ImageTextBox.Text != info.Image.ToString()) ImageTextBox.Text = string.Empty;
                if (DuraTextBox.Text != info.Durability.ToString()) DuraTextBox.Text = string.Empty;
                if (ITypeComboBox.SelectedItem == null || (ItemType)ITypeComboBox.SelectedItem != info.Type) ITypeComboBox.SelectedItem = null;
                if (ISetComboBox.SelectedItem == null || (ItemSet)ISetComboBox.SelectedItem != info.Set) ISetComboBox.SelectedItem = null;
                if (ShapeTextBox.Text != info.Shape.ToString()) ShapeTextBox.Text = string.Empty;
                if (SSizeTextBox.Text != info.StackSize.ToString()) SSizeTextBox.Text = string.Empty;
                if (PriceTextBox.Text != info.Price.ToString()) PriceTextBox.Text = string.Empty;
                if (RTypeComboBox.SelectedItem == null || (RequiredType)RTypeComboBox.SelectedItem != info.RequiredType) RTypeComboBox.SelectedItem = null;
                if (RAmountTextBox.Text != info.RequiredAmount.ToString()) RAmountTextBox.Text = string.Empty;
                if (RClassComboBox.SelectedItem == null || (RequiredClass)RClassComboBox.SelectedItem != info.RequiredClass) RClassComboBox.SelectedItem = null;
                if (RGenderComboBox.SelectedItem == null || (RequiredGender)RGenderComboBox.SelectedItem != info.RequiredGender) RGenderComboBox.SelectedItem = null;
                if (LightTextBox.Text != info.Light.ToString()) LightTextBox.Text = string.Empty;

                if (MinACTextBox.Text != info.MinAC.ToString()) MinACTextBox.Text = string.Empty;
                if (MaxACTextBox.Text != info.MaxAC.ToString()) MaxACTextBox.Text = string.Empty;
                if (MinMACTextBox.Text != info.MinMAC.ToString()) MinMACTextBox.Text = string.Empty;
                if (MaxMACTextBox.Text != info.MaxMAC.ToString()) MaxMACTextBox.Text = string.Empty;
                if (MinDCTextBox.Text != info.MinDC.ToString()) MinDCTextBox.Text = string.Empty;
                if (MaxDCTextBox.Text != info.MaxDC.ToString()) MaxDCTextBox.Text = string.Empty;
                if (MinMCTextBox.Text != info.MinMC.ToString()) MinMCTextBox.Text = string.Empty;
                if (MaxMCTextBox.Text != info.MaxMC.ToString()) MaxMCTextBox.Text = string.Empty;
                if (MinSCTextBox.Text != info.MinSC.ToString()) MinSCTextBox.Text = string.Empty;
                if (MaxSCTextBox.Text != info.MaxSC.ToString()) MaxSCTextBox.Text = string.Empty;
                if (HPTextBox.Text != info.HP.ToString()) HPTextBox.Text = string.Empty;
                if (MPTextBox.Text != info.MP.ToString()) MPTextBox.Text = string.Empty;
                if (AccuracyTextBox.Text != info.Accuracy.ToString()) AccuracyTextBox.Text = string.Empty;
                if (AgilityTextBox.Text != info.Agility.ToString()) AgilityTextBox.Text = string.Empty;
                if (ASpeedTextBox.Text != info.AttackSpeed.ToString()) ASpeedTextBox.Text = string.Empty;
                if (LuckTextBox.Text != info.Luck.ToString()) LuckTextBox.Text = string.Empty;

                if (WWeightTextBox.Text != info.WearWeight.ToString()) WWeightTextBox.Text = string.Empty;
                if (HWeightTextBox.Text != info.HandWeight.ToString()) HWeightTextBox.Text = string.Empty;
                if (BWeightText.Text != info.BagWeight.ToString()) BWeightText.Text = string.Empty;

                if (StartItemCheckBox.Checked != info.StartItem) StartItemCheckBox.CheckState = CheckState.Indeterminate;
                if (EffectTextBox.Text != info.Effect.ToString()) EffectTextBox.Text = string.Empty;
            }
        }

        private void RefreshItemList()
        {
            ItemInfoListBox.SelectedIndexChanged -= ItemInfoListBox_SelectedIndexChanged;

            List<bool> selected = new List<bool>();

            for (int i = 0; i < ItemInfoListBox.Items.Count; i++) selected.Add(ItemInfoListBox.GetSelected(i));
            ItemInfoListBox.Items.Clear();
            for (int i = 0; i < Envir.ItemInfoList.Count; i++) ItemInfoListBox.Items.Add(Envir.ItemInfoList[i]);
            for (int i = 0; i < selected.Count; i++) ItemInfoListBox.SetSelected(i, selected[i]);

            ItemInfoListBox.SelectedIndexChanged += ItemInfoListBox_SelectedIndexChanged;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (ITypeFilterComboBox.SelectedIndex == ITypeFilterComboBox.Items.Count - 1)
            {
                Envir.CreateItemInfo();
                ITypeFilterComboBox.SelectedIndex = ITypeFilterComboBox.Items.Count - 1;
            }
            else
            {
                Envir.CreateItemInfo((ItemType)ITypeFilterComboBox.SelectedItem);
            }

            UpdateInterface(true);
        }

        private void ItemInfoListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateInterface();
        }

        private void ITypeFilterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateInterface(true);
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (_selectedItemInfos.Count == 0) return;

            if (MessageBox.Show("Are you sure you want to remove the selected Items?", "Remove Items?", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            for (int i = 0; i < _selectedItemInfos.Count; i++) Envir.Remove(_selectedItemInfos[i]);

            if (Envir.ItemInfoList.Count == 0) Envir.ItemIndex = 0;

            UpdateInterface(true);
        }
        private void ItemNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].Name = ActiveControl.Text;

            RefreshItemList();
        }
        private void ITypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].Type = (ItemType)ITypeComboBox.SelectedItem;
        }
        private void RTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].RequiredType = (RequiredType) RTypeComboBox.SelectedItem;
        }
        private void RGenderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].RequiredGender = (RequiredGender)RGenderComboBox.SelectedItem;
        }
        private void RClassComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].RequiredClass = (RequiredClass)RClassComboBox.SelectedItem;
        }
        private void StartItemCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].StartItem = StartItemCheckBox.Checked;
        }
        private void WeightTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].Weight = temp;
        }
        private void ImageTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            ushort temp;

            if (!ushort.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].Image = temp;
        }
        private void DuraTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            ushort temp;

            if (!ushort.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].Durability = temp;
        }
        private void ShapeTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            sbyte temp;

            if (!sbyte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].Shape = temp;
        }
        private void SSizeTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            uint temp;

            if (!uint.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].StackSize = temp;
        }
        private void PriceTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            uint temp;

            if (!uint.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].Price = temp;
        }
        private void RAmountTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].RequiredAmount = temp;
        }
        private void LightTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].Light = temp;
        }
        private void MinACTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].MinAC = temp;
        }
        private void MaxACTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].MaxAC = temp;
        }
        private void MinMACTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].MinMAC = temp;
        }
        private void MaxMACTextBox_TextChanged(object sender, EventArgs e)
        {

            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].MaxMAC = temp;
        }
        private void MinDCTextBox_TextChanged(object sender, EventArgs e)
        {

            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].MinDC = temp;
        }
        private void MaxDCTextBox_TextChanged(object sender, EventArgs e)
        {

            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].MaxDC = temp;
        }
        private void MinMCTextBox_TextChanged(object sender, EventArgs e)
        {

            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].MinMC = temp;
        }
        private void MaxMCTextBox_TextChanged(object sender, EventArgs e)
        {

            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].MaxMC = temp;
        }
        private void MinSCTextBox_TextChanged(object sender, EventArgs e)
        {

            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].MinSC = temp;
        }
        private void MaxSCTextBox_TextChanged(object sender, EventArgs e)
        {

            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].MaxSC = temp;
        }
        private void HPTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].HP = temp;
        }
        private void MPTextBox_TextChanged(object sender, EventArgs e)
        {

            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].MP = temp;
        }
        private void AccuracyTextBox_TextChanged(object sender, EventArgs e)
        {

            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].Accuracy = temp;
        }
        private void AgilityTextBox_TextChanged(object sender, EventArgs e)
        {

            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].Agility = temp;
        }
        private void ASpeedTextBox_TextChanged(object sender, EventArgs e)
        {

            if (ActiveControl != sender) return;

            sbyte temp;

            if (!sbyte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].AttackSpeed = temp;
        }
        private void LuckTextBox_TextChanged(object sender, EventArgs e)
        {

            if (ActiveControl != sender) return;

            sbyte temp;

            if (!sbyte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].Luck = temp;
        }
        private void BWeightText_TextChanged(object sender, EventArgs e)
        {

            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].BagWeight = temp;
        }
        private void HWeightTextBox_TextChanged(object sender, EventArgs e)
        {

            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].HandWeight = temp;
        }
        private void WWeightTextBox_TextChanged(object sender, EventArgs e)
        {

            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].WearWeight = temp;
        }
        private void EffectTextBox_TextChanged(object sender, EventArgs e)
        {

            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].Effect = temp;
        }

        private void ItemInfoForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Envir.SaveDB();
        }

        private void PasteButton_Click(object sender, EventArgs e)
        {
            string data = Clipboard.GetText();

            if (!data.StartsWith("Item", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Cannot Paste, Copied data is not Item Information.");
                return;
            }


            string[] items = data.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);


            for (int i = 1; i < items.Length; i++)
            {
                ItemInfo info = ItemInfo.FromText(items[i]);

                if (info == null) continue;
                info.Index = ++Envir.ItemIndex;
                Envir.ItemInfoList.Add(info);

            }

            UpdateInterface();
        }

        private void CopyMButton_Click(object sender, EventArgs e)
        {

        }

        private void ExportAllButton_Click(object sender, EventArgs e)
        {
            ExportItems(Envir.ItemInfoList);
        }

        private void ExportSelectedButton_Click(object sender, EventArgs e)
        {
            var list = ItemInfoListBox.SelectedItems.Cast<ItemInfo>().ToList();

            ExportItems(list);
        }

        private void ExportItems(IEnumerable<ItemInfo> items)
        {
            var itemInfos = items as ItemInfo[] ?? items.ToArray();
            var list = itemInfos.Select(item => item.ToText()).ToList();

            File.WriteAllLines(ItemListPath, list);

            MessageBox.Show(itemInfos.Count() + " Items have been exported");
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            string data;
            using (var sr = new StreamReader(ItemListPath))
            {
                data = sr.ReadToEnd();
            }

            var items = data.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            var count = 0;
            foreach (var info in items.Select(ItemInfo.FromText).Where(info => info != null))
            {
                count++;
                info.Index = ++Envir.ItemIndex;
                Envir.ItemInfoList.Add(info);
            }

            MessageBox.Show(count + " Items have been imported");
            UpdateInterface(true);
        }

        private void ISetComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedItemInfos.Count; i++)
                _selectedItemInfos[i].Set = (ItemSet)ISetComboBox.SelectedItem;
        }
    }
}
