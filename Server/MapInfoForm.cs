using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Server.MirDatabase;
using Server.MirEnvir;

namespace Server
{
    public partial class MapInfoForm : Form
    {
        public Envir Envir
        {
            get { return SMain.EditEnvir; }
        }

        private List<MapInfo> _selectedMapInfos;
        private List<SafeZoneInfo> _selectedSafeZoneInfos;
        private List<RespawnInfo> _selectedRespawnInfos;
        private List<NPCInfo> _selectedNPCInfos;
        private List<MovementInfo> _selectedMovementInfos;
        private MapInfo _info;

        public MapInfoForm()
        {
            InitializeComponent();

            LightsComboBox.Items.AddRange(Enum.GetValues(typeof(LightSetting)).Cast<object>().ToArray());
            for (int i = 0; i < Envir.MonsterInfoList.Count; i++) MonsterInfoComboBox.Items.Add(Envir.MonsterInfoList[i]);
            UpdateInterface();
        }
        private void MapInfoForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Envir.SaveDB();
        }


        private void UpdateInterface()
        {
            //List<MapInfo> orderedMapInfoList = Envir.MapInfoList.OrderBy(m => m.Title).ToList();

            if (MapInfoListBox.Items.Count != Envir.MapInfoList.Count)
            {
                MapInfoListBox.Items.Clear();
                DestMapComboBox.Items.Clear();

                for (int i = 0; i < Envir.MapInfoList.Count; i++)
                {
                    MapInfoListBox.Items.Add(Envir.MapInfoList[i]);
                    DestMapComboBox.Items.Add(Envir.MapInfoList[i]);
                }
            }
            
            _selectedMapInfos = MapInfoListBox.SelectedItems.Cast<MapInfo>().ToList();

            if (_selectedMapInfos == null || _selectedMapInfos.Count == 0)
            {
                tabPage1.Show();
                MapTabs.Enabled = false;
                MapIndexTextBox.Text = string.Empty;
                FileNameTextBox.Text = string.Empty;
                MapNameTextBox.Text = string.Empty;
                MiniMapTextBox.Text = string.Empty;
                BigMapTextBox.Text = string.Empty;
                LightsComboBox.SelectedItem = null;

                NoTeleportCheckbox.Checked = false;
                NoReconnectCheckbox.Checked = false;
                NoRandomCheckbox.Checked = false;
                NoEscapeCheckbox.Checked = false;
                NoRecallCheckbox.Checked = false;
                NoDrugCheckbox.Checked = false;

                NoPositionCheckbox.Checked = false;
                NoThrowItemCheckbox.Checked = false;
                NoDropPlayerCheckbox.Checked = false;
                NoDropMonsterCheckbox.Checked = false;
                NoNamesCheckbox.Checked = false;

                FightCheckbox.Checked = false;
                FireCheckbox.Checked = false;
                LightningCheckbox.Checked = false;
                NoReconnectTextbox.Text = string.Empty;
                FireTextbox.Text = string.Empty;
                LightningTextbox.Text = string.Empty;
                return;
            }


            MapInfo mi = _selectedMapInfos[0];

            MapTabs.Enabled = true;

            MapIndexTextBox.Text = mi.Index.ToString();
            FileNameTextBox.Text = mi.FileName;
            MapNameTextBox.Text = mi.Title;
            MiniMapTextBox.Text = mi.MiniMap.ToString();
            BigMapTextBox.Text = mi.BigMap.ToString();
            LightsComboBox.SelectedItem = mi.Light;

            //map attributes
            NoTeleportCheckbox.Checked = mi.NoTeleport;
            NoReconnectCheckbox.Checked = mi.NoReconnect;
            NoReconnectTextbox.Text = mi.NoReconnectMap;
            NoRandomCheckbox.Checked = mi.NoRandom;
            NoEscapeCheckbox.Checked = mi.NoEscape;
            NoRecallCheckbox.Checked = mi.NoRecall;
            NoDrugCheckbox.Checked = mi.NoDrug;
            NoPositionCheckbox.Checked = mi.NoPosition;
            NoThrowItemCheckbox.Checked = mi.NoThrowItem;
            NoDropPlayerCheckbox.Checked = mi.NoDropPlayer;
            NoDropMonsterCheckbox.Checked = mi.NoDropMonster;
            NoNamesCheckbox.Checked = mi.NoNames;
            FightCheckbox.Checked = mi.Fight;
            FireCheckbox.Checked = mi.Fire;
            FireTextbox.Text = mi.FireDamage.ToString();
            LightningCheckbox.Checked = mi.Lightning;                      
            LightningTextbox.Text = mi.LightningDamage.ToString();

            for (int i = 1; i < _selectedMapInfos.Count; i++)
            {
                mi = _selectedMapInfos[i];

                if (MapIndexTextBox.Text != mi.Index.ToString()) MapIndexTextBox.Text = string.Empty;
                if (FileNameTextBox.Text != mi.FileName) FileNameTextBox.Text = string.Empty;
                if (MapNameTextBox.Text != mi.Title) MapNameTextBox.Text = string.Empty;
                if (MiniMapTextBox.Text != mi.MiniMap.ToString()) MiniMapTextBox.Text = string.Empty;
                if (BigMapTextBox.Text != mi.BigMap.ToString()) BigMapTextBox.Text = string.Empty;
                if (LightsComboBox.SelectedItem == null || (LightSetting)LightsComboBox.SelectedItem != mi.Light) LightsComboBox.SelectedItem = null;

                //map attributes
                if (NoTeleportCheckbox.Checked != mi.NoTeleport) NoTeleportCheckbox.Checked = false;
                if (NoReconnectCheckbox.Checked != mi.NoReconnect) NoReconnectCheckbox.Checked = false;
                if (NoReconnectTextbox.Text != mi.NoReconnectMap) NoReconnectTextbox.Text = string.Empty;
                if (NoRandomCheckbox.Checked != mi.NoRandom) NoRandomCheckbox.Checked = false;
                if (NoEscapeCheckbox.Checked != mi.NoEscape) NoEscapeCheckbox.Checked = false;
                if (NoRecallCheckbox.Checked != mi.NoRecall) NoRecallCheckbox.Checked = false;
                if (NoDrugCheckbox.Checked != mi.NoDrug) NoDrugCheckbox.Checked = false;
                if (NoPositionCheckbox.Checked != mi.NoPosition) NoPositionCheckbox.Checked = false;
                if (NoThrowItemCheckbox.Checked != mi.NoThrowItem) NoThrowItemCheckbox.Checked = false;
                if (NoDropPlayerCheckbox.Checked != mi.NoDropPlayer) NoDropPlayerCheckbox.Checked = false;
                if (NoDropMonsterCheckbox.Checked != mi.NoDropMonster) NoDropMonsterCheckbox.Checked = false;
                if (NoNamesCheckbox.Checked != mi.NoNames) NoNamesCheckbox.Checked = false;
                if (FightCheckbox.Checked != mi.Fight) FightCheckbox.Checked = false;
                if (FireCheckbox.Checked != mi.Fire) FireCheckbox.Checked = false;
                if (FireTextbox.Text != mi.FireDamage.ToString()) FireTextbox.Text = string.Empty;
                if (LightningCheckbox.Checked != mi.Lightning) LightningCheckbox.Checked = false;                             
                if (LightningTextbox.Text != mi.LightningDamage.ToString()) LightningTextbox.Text = string.Empty;
            }

            UpdateSafeZoneInterface();
            UpdateRespawnInterface();
            UpdateNPCInterface();
            UpdateMovementInterface();
        }
        private void UpdateSafeZoneInterface()
        {
            if (_selectedMapInfos.Count != 1)
            {
                SafeZoneInfoListBox.Items.Clear();
                if (_selectedSafeZoneInfos != null && _selectedSafeZoneInfos.Count > 0)
                    _selectedSafeZoneInfos.Clear();
                _info = null;

                SafeZoneInfoPanel.Enabled = false;

                SZXTextBox.Text = string.Empty;
                SZYTextBox.Text = string.Empty;
                StartPointCheckBox.CheckState = CheckState.Unchecked;
                return;
            }

            if (_info != _selectedMapInfos[0])
            {
                SafeZoneInfoListBox.Items.Clear();
                _info = _selectedMapInfos[0];
            }

            if (SafeZoneInfoListBox.Items.Count != _info.SafeZones.Count)
            {
                SafeZoneInfoListBox.Items.Clear();
                for (int i = 0; i < _info.SafeZones.Count; i++) SafeZoneInfoListBox.Items.Add(_info.SafeZones[i]);
            }
            _selectedSafeZoneInfos = SafeZoneInfoListBox.SelectedItems.Cast<SafeZoneInfo>().ToList();

            if (_selectedSafeZoneInfos.Count == 0)
            {
                SafeZoneInfoPanel.Enabled = false;

                SZXTextBox.Text = string.Empty;
                SZYTextBox.Text = string.Empty;
                StartPointCheckBox.CheckState = CheckState.Unchecked;
                return;
            }

            SafeZoneInfo info = _selectedSafeZoneInfos[0];
            SafeZoneInfoPanel.Enabled = true;

            SZXTextBox.Text = info.Location.X.ToString();
            SZYTextBox.Text = info.Location.Y.ToString();
            StartPointCheckBox.CheckState = info.StartPoint ? CheckState.Checked : CheckState.Unchecked;
            SizeTextBox.Text = info.Size.ToString();


            for (int i = 1; i < _selectedSafeZoneInfos.Count; i++)
            {
                info = _selectedSafeZoneInfos[i];

                if (SZXTextBox.Text != info.Location.X.ToString()) SZXTextBox.Text = string.Empty;
                if (SZYTextBox.Text != info.Location.Y.ToString()) SZYTextBox.Text = string.Empty;
                if (StartPointCheckBox.Checked != info.StartPoint) StartPointCheckBox.CheckState = CheckState.Indeterminate;
                if (SizeTextBox.Text != info.Size.ToString()) SizeTextBox.Text = string.Empty;
            }
        }
        private void UpdateRespawnInterface()
        {
            if (_selectedMapInfos.Count != 1)
            {
                RespawnInfoListBox.Items.Clear();
                if (_selectedRespawnInfos != null && _selectedRespawnInfos.Count > 0)
                    _selectedRespawnInfos.Clear();
                _info = null;

                RespawnInfoPanel.Enabled = false;

                MonsterInfoComboBox.SelectedItem = null;
                RXTextBox.Text = string.Empty;
                RYTextBox.Text = string.Empty;
                CountTextBox.Text = string.Empty;
                SpreadTextBox.Text = string.Empty;
                DelayTextBox.Text = string.Empty;
                DirectionTextBox.Text = string.Empty;
                return;
            }

            if (_info != _selectedMapInfos[0])
            {
                RespawnInfoListBox.Items.Clear();
                _info = _selectedMapInfos[0];
            }

            if (RespawnInfoListBox.Items.Count != _info.Respawns.Count)
            {
                RespawnInfoListBox.Items.Clear();
                for (int i = 0; i < _info.Respawns.Count; i++) RespawnInfoListBox.Items.Add(_info.Respawns[i]);
            }
            _selectedRespawnInfos = RespawnInfoListBox.SelectedItems.Cast<RespawnInfo>().ToList();

            if (_selectedRespawnInfos.Count == 0)
            {
                RespawnInfoPanel.Enabled = false;

                MonsterInfoComboBox.SelectedItem = null;
                RXTextBox.Text = string.Empty;
                RYTextBox.Text = string.Empty;
                CountTextBox.Text = string.Empty;
                SpreadTextBox.Text = string.Empty;
                DelayTextBox.Text = string.Empty;
                DirectionTextBox.Text = string.Empty;
                return;
            }

            RespawnInfo info = _selectedRespawnInfos[0];
            RespawnInfoPanel.Enabled = true;

            MonsterInfoComboBox.SelectedItem = Envir.MonsterInfoList.FirstOrDefault(x => x.Index == info.MonsterIndex);
            RXTextBox.Text = info.Location.X.ToString();
            RYTextBox.Text = info.Location.Y.ToString();
            CountTextBox.Text = info.Count.ToString();
            SpreadTextBox.Text = info.Spread.ToString();
            DelayTextBox.Text = info.Delay.ToString();
            DirectionTextBox.Text = info.Direction.ToString();


            for (int i = 1; i < _selectedRespawnInfos.Count; i++)
            {
                info = _selectedRespawnInfos[i];

                if (MonsterInfoComboBox.SelectedItem != Envir.MonsterInfoList.FirstOrDefault(x => x.Index == info.MonsterIndex)) MonsterInfoComboBox.SelectedItem = null;
                if (RXTextBox.Text != info.Location.X.ToString()) RXTextBox.Text = string.Empty;
                if (RYTextBox.Text != info.Location.Y.ToString()) RYTextBox.Text = string.Empty;
                if (CountTextBox.Text != info.Count.ToString()) CountTextBox.Text = string.Empty;
                if (SpreadTextBox.Text != info.Spread.ToString()) SpreadTextBox.Text = string.Empty;
                if (DelayTextBox.Text != info.Delay.ToString()) DelayTextBox.Text = string.Empty;
                if (DirectionTextBox.Text != info.Direction.ToString()) DirectionTextBox.Text = string.Empty;
            }
        }
        private void UpdateNPCInterface()
        {
            if (_selectedMapInfos.Count != 1)
            {
                NPCInfoListBox.Items.Clear();
                if (_selectedNPCInfos != null && _selectedNPCInfos.Count > 0)
                    _selectedNPCInfos.Clear();
                _info = null;

                NPCInfoPanel.Enabled = false;

                NFileNameTextBox.Text = string.Empty;
                NNameTextBox.Text = string.Empty;
                NXTextBox.Text = string.Empty;
                NYTextBox.Text = string.Empty;
                NImageTextBox.Text = string.Empty;
                NRateTextBox.Text = string.Empty;
                return;
            }

            if (_info != _selectedMapInfos[0])
            {
                NPCInfoListBox.Items.Clear();
                _info = _selectedMapInfos[0];
            }

            if (NPCInfoListBox.Items.Count != _info.NPCs.Count)
            {
                NPCInfoListBox.Items.Clear();
                for (int i = 0; i < _info.NPCs.Count; i++) NPCInfoListBox.Items.Add(_info.NPCs[i]);
            }
            _selectedNPCInfos = NPCInfoListBox.SelectedItems.Cast<NPCInfo>().ToList();

            if (_selectedNPCInfos.Count == 0)
            {
                NPCInfoPanel.Enabled = false;

                NFileNameTextBox.Text = string.Empty;
                NNameTextBox.Text = string.Empty;
                NXTextBox.Text = string.Empty;
                NYTextBox.Text = string.Empty;
                NImageTextBox.Text = string.Empty;
                NRateTextBox.Text = string.Empty;
                return;
            }

            NPCInfo info = _selectedNPCInfos[0];

            NPCInfoPanel.Enabled = true;

            NFileNameTextBox.Text = info.FileName;
            NNameTextBox.Text = info.Name;
            NXTextBox.Text = info.Location.X.ToString();
            NYTextBox.Text = info.Location.Y.ToString();
            NImageTextBox.Text = info.Image.ToString();
            NRateTextBox.Text = info.Rate.ToString();


            for (int i = 1; i < _selectedNPCInfos.Count; i++)
            {
                info = _selectedNPCInfos[i];

                if (NFileNameTextBox.Text != info.FileName) NFileNameTextBox.Text = string.Empty;
                if (NNameTextBox.Text != info.Name) NNameTextBox.Text = string.Empty;
                if (NXTextBox.Text != info.Location.X.ToString()) NXTextBox.Text = string.Empty;

                if (NYTextBox.Text != info.Location.Y.ToString()) NYTextBox.Text = string.Empty;
                if (NImageTextBox.Text != info.Image.ToString()) NImageTextBox.Text = string.Empty;
                if (NRateTextBox.Text != info.Rate.ToString()) NRateTextBox.Text = string.Empty;
            }

        }
        private void UpdateMovementInterface()
        {
            if (_selectedMapInfos.Count != 1)
            {
                MovementInfoListBox.Items.Clear();
                if (_selectedMovementInfos != null && _selectedMovementInfos.Count > 0)
                    _selectedMovementInfos.Clear();
                _info = null;

                MovementInfoPanel.Enabled = false;

                SourceXTextBox.Text = string.Empty;
                SourceYTextBox.Text = string.Empty;
                DestMapComboBox.SelectedItem = null;
                DestXTextBox.Text = string.Empty;
                DestYTextBox.Text = string.Empty;
                return;
            }

            if (_info != _selectedMapInfos[0])
            {
                MovementInfoListBox.Items.Clear();
                _info = _selectedMapInfos[0];
            }

            if (MovementInfoListBox.Items.Count != _info.Movements.Count)
            {
                MovementInfoListBox.Items.Clear();
                for (int i = 0; i < _info.Movements.Count; i++) MovementInfoListBox.Items.Add(_info.Movements[i]);
            }
            _selectedMovementInfos = MovementInfoListBox.SelectedItems.Cast<MovementInfo>().ToList();

            if (_selectedMovementInfos.Count == 0)
            {
                MovementInfoPanel.Enabled = false;

                SourceXTextBox.Text = string.Empty;
                SourceYTextBox.Text = string.Empty;
                NeedHoleMCheckBox.Checked = false;
                DestMapComboBox.SelectedItem = null;
                DestXTextBox.Text = string.Empty;
                DestYTextBox.Text = string.Empty;
                return;
            }

            MovementInfo info = _selectedMovementInfos[0];

            MovementInfoPanel.Enabled = true;

            SourceXTextBox.Text = info.Source.X.ToString();
            SourceYTextBox.Text = info.Destination.Y.ToString();
            NeedHoleMCheckBox.Checked = info.NeedHole;
            DestMapComboBox.SelectedItem = Envir.MapInfoList.FirstOrDefault(x => x.Index == info.MapIndex);
            DestXTextBox.Text = info.Destination.X.ToString();
            DestYTextBox.Text = info.Destination.Y.ToString();
            

            for (int i = 1; i < _selectedMovementInfos.Count; i++)
            {
                info = _selectedMovementInfos[i];

                SourceXTextBox.Text = info.Source.X.ToString();
                SourceYTextBox.Text = info.Source.Y.ToString();
                DestMapComboBox.SelectedItem = Envir.MapInfoList.FirstOrDefault(x => x.Index == info.MapIndex);
                DestXTextBox.Text = info.Destination.X.ToString();
                DestYTextBox.Text = info.Destination.Y.ToString();

                if (SourceXTextBox.Text != info.Source.X.ToString()) SourceXTextBox.Text = string.Empty;
                if (SourceYTextBox.Text != info.Source.Y.ToString()) SourceYTextBox.Text = string.Empty;

                if (DestMapComboBox.SelectedItem != Envir.MapInfoList.FirstOrDefault(x => x.Index == info.MapIndex)) DestMapComboBox.SelectedItem = null;

                if (DestXTextBox.Text != info.Destination.X.ToString()) DestXTextBox.Text = string.Empty;
                if (DestYTextBox.Text != info.Destination.Y.ToString()) DestYTextBox.Text = string.Empty;
            }

        }

        private void RefreshMapList()
        {
            MapInfoListBox.SelectedIndexChanged -= MapInfoListBox_SelectedIndexChanged;

            List<bool> selected = new List<bool>();

            for (int i = 0; i < MapInfoListBox.Items.Count; i++) selected.Add(MapInfoListBox.GetSelected(i));
            MapInfoListBox.Items.Clear();
            for (int i = 0; i < Envir.MapInfoList.Count; i++) MapInfoListBox.Items.Add(Envir.MapInfoList[i]);
            for (int i = 0; i < selected.Count; i++) MapInfoListBox.SetSelected(i, selected[i]);

            MapInfoListBox.SelectedIndexChanged += MapInfoListBox_SelectedIndexChanged;
        }
        private void RefreshSafeZoneList()
        {
            SafeZoneInfoListBox.SelectedIndexChanged -= SafeZoneInfoListBox_SelectedIndexChanged;

            List<bool> selected = new List<bool>();

            for (int i = 0; i < SafeZoneInfoListBox.Items.Count; i++) selected.Add(SafeZoneInfoListBox.GetSelected(i));
            SafeZoneInfoListBox.Items.Clear();
            for (int i = 0; i < _info.SafeZones.Count; i++) SafeZoneInfoListBox.Items.Add(_info.SafeZones[i]);
            for (int i = 0; i < selected.Count; i++) SafeZoneInfoListBox.SetSelected(i, selected[i]);

            SafeZoneInfoListBox.SelectedIndexChanged += SafeZoneInfoListBox_SelectedIndexChanged;
        }
        private void RefreshRespawnList()
        {
            RespawnInfoListBox.SelectedIndexChanged -= RespawnInfoListBox_SelectedIndexChanged;

            List<bool> selected = new List<bool>();

            for (int i = 0; i < RespawnInfoListBox.Items.Count; i++) selected.Add(RespawnInfoListBox.GetSelected(i));
            RespawnInfoListBox.Items.Clear();
            for (int i = 0; i < _info.Respawns.Count; i++) RespawnInfoListBox.Items.Add(_info.Respawns[i]);
            for (int i = 0; i < selected.Count; i++) RespawnInfoListBox.SetSelected(i, selected[i]);

            RespawnInfoListBox.SelectedIndexChanged += RespawnInfoListBox_SelectedIndexChanged;
        }
        private void RefreshNPCList()
        {
            NPCInfoListBox.SelectedIndexChanged -= NPCInfoListBox_SelectedIndexChanged;

            List<bool> selected = new List<bool>();

            for (int i = 0; i < NPCInfoListBox.Items.Count; i++) selected.Add(NPCInfoListBox.GetSelected(i));
            NPCInfoListBox.Items.Clear();
            for (int i = 0; i < _info.NPCs.Count; i++) NPCInfoListBox.Items.Add(_info.NPCs[i]);
            for (int i = 0; i < selected.Count; i++) NPCInfoListBox.SetSelected(i, selected[i]);

            NPCInfoListBox.SelectedIndexChanged += NPCInfoListBox_SelectedIndexChanged;
        }
        private void RefreshMovementList()
        {
            MovementInfoListBox.SelectedIndexChanged -= MovementInfoListBox_SelectedIndexChanged;

            List<bool> selected = new List<bool>();

            for (int i = 0; i < MovementInfoListBox.Items.Count; i++) selected.Add(MovementInfoListBox.GetSelected(i));
            MovementInfoListBox.Items.Clear();
            for (int i = 0; i < _info.Movements.Count; i++) MovementInfoListBox.Items.Add(_info.Movements[i]);
            for (int i = 0; i < selected.Count; i++) MovementInfoListBox.SetSelected(i, selected[i]);

            MovementInfoListBox.SelectedIndexChanged += MovementInfoListBox_SelectedIndexChanged;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            Envir.CreateMapInfo();
            UpdateInterface();
        }
        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (_selectedMapInfos.Count == 0) return;

            if (MessageBox.Show("Are you sure you want to remove the selected maps?", "Remove Maps?", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            for (int i = 0; i < _selectedMapInfos.Count; i++) Envir.Remove(_selectedMapInfos[i]);

            if (Envir.MapInfoList.Count == 0) Envir.MapIndex = 0;

            MapTabs.SelectTab(0);

            UpdateInterface();
        }
        private void MapInfoListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SafeZoneInfoListBox.Items.Clear();
            RespawnInfoListBox.Items.Clear();
            MovementInfoListBox.Items.Clear();
            NPCInfoListBox.Items.Clear();
            UpdateInterface();
        }
        private void FileNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;
            
            for (int i = 0; i < _selectedMapInfos.Count; i++)
                _selectedMapInfos[i].FileName = ActiveControl.Text;
        }
        private void MapNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedMapInfos.Count; i++)
                _selectedMapInfos[i].Title = ActiveControl.Text;

            RefreshMapList();
        }
        private void MiniMapTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            ushort temp;

            if (!ushort.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedMapInfos.Count; i++)
                _selectedMapInfos[i].MiniMap = temp;
        }
        private void LightsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedMapInfos.Count; i++)
                _selectedMapInfos[i].Light = (LightSetting)LightsComboBox.SelectedItem;
        }


        private void AddSZButton_Click(object sender, EventArgs e)
        {
            if (_info == null) return;

            _info.CreateSafeZone();
            UpdateSafeZoneInterface();
        }
        private void RemoveSZButton_Click(object sender, EventArgs e)
        {
            if(_selectedSafeZoneInfos.Count == 0) return;

            if (MessageBox.Show("Are you sure you want to remove the selected SafeZones?", "Remove SafeZones?", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            for (int i = 0; i < _selectedSafeZoneInfos.Count; i++) _info.SafeZones.Remove(_selectedSafeZoneInfos[i]);

            UpdateSafeZoneInterface();
        }
        private void SafeZoneInfoListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSafeZoneInterface();
        }
        private void SZXTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            int temp;

            if (!int.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedSafeZoneInfos.Count; i++)
                _selectedSafeZoneInfos[i].Location.X = temp;

            RefreshSafeZoneList();
        }
        private void SZYTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            int temp;

            if (!int.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedSafeZoneInfos.Count; i++)
                _selectedSafeZoneInfos[i].Location.Y = temp;

            RefreshSafeZoneList();
        }
        private void SizeTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            ushort temp;

            if (!ushort.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedSafeZoneInfos.Count; i++)
                _selectedSafeZoneInfos[i].Size = temp;
        }
        private void StartPointCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedSafeZoneInfos.Count; i++)
                _selectedSafeZoneInfos[i].StartPoint = StartPointCheckBox.Checked;

            RefreshSafeZoneList();
        }



        private void AddRButton_Click(object sender, EventArgs e)
        {
            if (_info == null) return;

            _info.CreateRespawnInfo();
            UpdateRespawnInterface();
        }
        private void RemoveRButton_Click(object sender, EventArgs e)
        {
            if (_selectedRespawnInfos.Count == 0) return;

            if (MessageBox.Show("Are you sure you want to remove the selected Respawns?", "Remove Respawns?", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            for (int i = 0; i < _selectedRespawnInfos.Count; i++) _info.Respawns.Remove(_selectedRespawnInfos[i]);

            UpdateRespawnInterface();
        }
        private void RespawnInfoListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateRespawnInterface();
        }
        private void MonsterInfoComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            MonsterInfo info = MonsterInfoComboBox.SelectedItem as MonsterInfo;

            if (info == null) return;

            for (int i = 0; i < _selectedRespawnInfos.Count; i++)
                    _selectedRespawnInfos[i].MonsterIndex = info.Index;

            RefreshRespawnList();

        }
        private void RXTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            int temp;

            if (!int.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedRespawnInfos.Count; i++)
                _selectedRespawnInfos[i].Location.X = temp;

            RefreshRespawnList();
        }
        private void RYTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            int temp;

            if (!int.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedRespawnInfos.Count; i++)
                _selectedRespawnInfos[i].Location.Y = temp;

            RefreshRespawnList();
        }
        private void CountTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            ushort temp;

            if (!ushort.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedRespawnInfos.Count; i++)
                _selectedRespawnInfos[i].Count = temp;

            RefreshRespawnList();
        }
        private void SpreadTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            ushort temp;

            if (!ushort.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedRespawnInfos.Count; i++)
                _selectedRespawnInfos[i].Spread = temp;

            RefreshRespawnList();
        }
        private void DelayTextBox_TextChanged(object sender, EventArgs e)
        {

            if (ActiveControl != sender) return;

            ushort temp;

            if (!ushort.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedRespawnInfos.Count; i++)
                _selectedRespawnInfos[i].Delay = temp;

            RefreshRespawnList();
        }
        private void DirectionTextBox_TextChanged(object sender, EventArgs e)
        {

            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedRespawnInfos.Count; i++)
                _selectedRespawnInfos[i].Direction = temp;

            RefreshRespawnList();
        }
        private void RPasteButton_Click(object sender, EventArgs e)
        {
            if (_info == null) return;
            
            string data = Clipboard.GetText();

            if (!data.StartsWith("Respawn", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Cannot Paste, Copied data is not Respawn Information.");
                return;
            }


            string[] respawns = data.Split(new[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);


            for (int i = 1; i < respawns.Length; i++)
            {
                RespawnInfo info = RespawnInfo.FromText(respawns[i]);
                
                if (info == null) continue;

                _info.Respawns.Add(info);
            }

            UpdateRespawnInterface();
        }
        //RCopy


        private void AddNButton_Click(object sender, EventArgs e)
        {
            if (_info == null) return;

            _info.CreateNPCInfo();
            UpdateNPCInterface();
        }
        private void RemoveNButton_Click(object sender, EventArgs e)
        {
            if (_selectedNPCInfos.Count == 0) return;

            if (MessageBox.Show("Are you sure you want to remove the selected NPCs?", "Remove NPCs?", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            for (int i = 0; i < _selectedNPCInfos.Count; i++) _info.NPCs.Remove(_selectedNPCInfos[i]);

            UpdateNPCInterface();
        }
        private void NPCInfoListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateNPCInterface();
        }
        private void NFileNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedMapInfos.Count; i++)
                _selectedNPCInfos[i].FileName = ActiveControl.Text;
        }
        private void NNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedMapInfos.Count; i++)
                _selectedNPCInfos[i].Name = ActiveControl.Text;

            RefreshNPCList();
        }
        private void NXTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            int temp;

            if (!int.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].Location.X = temp;

            RefreshNPCList();
        }
        private void NYTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            int temp;

            if (!int.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].Location.Y = temp;

            RefreshNPCList();
        }
        private void NImageTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].Image = temp;

        }
        private void NRateTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            ushort temp;

            if (!ushort.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].Rate = temp;
        }



        private void AddMButton_Click(object sender, EventArgs e)
        {
            if (_info == null) return;

            _info.CreateMovementInfo();
            UpdateMovementInterface();
        }
        private void RemoveMButton_Click(object sender, EventArgs e)
        {
            if (_selectedMovementInfos.Count == 0) return;

            if (MessageBox.Show("Are you sure you want to remove the selected Movements?", "Remove Movements?", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            for (int i = 0; i < _selectedMovementInfos.Count; i++) _info.Movements.Remove(_selectedMovementInfos[i]);

            UpdateMovementInterface();
        }
        private void SourceXTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            int temp;

            if (!int.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedMovementInfos.Count; i++)
                _selectedMovementInfos[i].Source.X = temp;

            RefreshMovementList();
        }
        private void SourceYTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            int temp;

            if (!int.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedMovementInfos.Count; i++)
                _selectedMovementInfos[i].Source.Y = temp;

            RefreshMovementList();
        }
        private void DestXTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            int temp;

            if (!int.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedMovementInfos.Count; i++)
                _selectedMovementInfos[i].Destination.X = temp;

            RefreshMovementList();
        }
        private void DestYTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            int temp;

            if (!int.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedMovementInfos.Count; i++)
                _selectedMovementInfos[i].Destination.Y = temp;

            RefreshMovementList();
        }
        private void DestMapComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            MapInfo info = DestMapComboBox.SelectedItem as MapInfo;

            if (info == null) return;

            for (int i = 0; i < _selectedMovementInfos.Count; i++)
                _selectedMovementInfos[i].MapIndex = info.Index;

            RefreshMovementList();

        }
        private void NeedHoleMCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedMovementInfos.Count; i++)
                _selectedMovementInfos[i].NeedHole = NeedHoleMCheckBox.Checked;
        }
        private void MovementInfoListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateMovementInterface();
        }

        private void PasteMapButton_Click(object sender, EventArgs e)
        {
            string data = Clipboard.GetText();

            if (!data.StartsWith("Map", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Cannot Paste, Copied data is not Map Information.");
                return;
            }


            string[] monsters = data.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);


            for (int i = 1; i < monsters.Length; i++)
                MapInfo.FromText(monsters[i]);

            UpdateInterface();
        }

        private void BigMapTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            ushort temp;

            if (!ushort.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedMapInfos.Count; i++)
                _selectedMapInfos[i].BigMap = temp;
        }



        private void NoTeleportCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedMapInfos.Count; i++)
                _selectedMapInfos[i].NoTeleport = NoTeleportCheckbox.Checked;
        }
        private void NoReconnectCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedMapInfos.Count; i++)
                _selectedMapInfos[i].NoReconnect = NoReconnectCheckbox.Checked;
        }
        private void NoReconnectTextbox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedMapInfos.Count; i++)
                _selectedMapInfos[i].NoReconnectMap = ActiveControl.Text;
        }
        private void NoRandomCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedMapInfos.Count; i++)
                _selectedMapInfos[i].NoRandom = NoRandomCheckbox.Checked;
        }
        private void NoEscapeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedMapInfos.Count; i++)
                _selectedMapInfos[i].NoEscape = NoEscapeCheckbox.Checked;
        }
        private void NoRecallCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedMapInfos.Count; i++)
                _selectedMapInfos[i].NoRecall = NoRecallCheckbox.Checked;
        }
        private void NoDrugCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedMapInfos.Count; i++)
                _selectedMapInfos[i].NoDrug = NoDrugCheckbox.Checked;
        }
        private void NoPositionCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedMapInfos.Count; i++)
                _selectedMapInfos[i].NoPosition = NoPositionCheckbox.Checked;
        }
        private void NoThrowItemCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedMapInfos.Count; i++)
                _selectedMapInfos[i].NoThrowItem = NoThrowItemCheckbox.Checked;
        }
        private void NoDropPlayerCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedMapInfos.Count; i++)
                _selectedMapInfos[i].NoDropPlayer = NoDropPlayerCheckbox.Checked;
        }
        private void NoDropMonsterCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedMapInfos.Count; i++)
                _selectedMapInfos[i].NoDropMonster = NoDropMonsterCheckbox.Checked;
        }
        private void NoNamesCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedMapInfos.Count; i++)
                _selectedMapInfos[i].NoNames = NoNamesCheckbox.Checked;
        }
        private void FightCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedMapInfos.Count; i++)
                _selectedMapInfos[i].Fight = FightCheckbox.Checked;
        }
        private void FireCheckbox_CheckStateChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedMapInfos.Count; i++)
                _selectedMapInfos[i].Fire = FireCheckbox.Checked;
        }
        private void FireTextbox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            int temp;

            if (!int.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedMapInfos.Count; i++)
                _selectedMapInfos[i].FireDamage = temp;
        }
        private void LightningCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedMapInfos.Count; i++)
                _selectedMapInfos[i].Lightning = LightningCheckbox.Checked;
        }
        private void LightningTextbox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            int temp;

            if (!int.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedMapInfos.Count; i++)
                _selectedMapInfos[i].LightningDamage = temp;
        }

        private void OpenNButton_Click(object sender, EventArgs e)
        {
            if (NFileNameTextBox.Text == string.Empty) return;

            var scriptPath = Settings.NPCPath + NFileNameTextBox.Text + ".txt";
            if (File.Exists(scriptPath))
                Process.Start(scriptPath);
            else
                MessageBox.Show("Script file could not be found"); 
        }
    }
}
