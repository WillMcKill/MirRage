using System;
using System.Drawing;
using System.Windows.Forms;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirObjects;
using Client.MirScenes;
using Client.MirSounds;
using C = ClientPackets;

namespace Client.MirControls
{
    public sealed class MirItemCell : MirImageControl
    {

        public UserItem Item
        {
            get
            {
                if (GridType == MirGridType.DropPanel)
                    return NPCDropDialog.TargetItem;

                if (GridType == MirGridType.TrustMerchant)
                    return TrustMerchantDialog.Selected != null ? TrustMerchantDialog.Selected.Listing.Item : null;

                if (ItemArray != null && _itemSlot >= 0 && _itemSlot < ItemArray.Length)
                    return ItemArray[_itemSlot];
                return null;
            }
            set
            {
                if (GridType == MirGridType.DropPanel)
                    NPCDropDialog.TargetItem = value;
                else if (ItemArray != null && _itemSlot >= 0 && _itemSlot < ItemArray.Length)
                    ItemArray[_itemSlot] = value;

                Redraw();
            }
        }

        public UserItem[] ItemArray
        {
            get
            {
                switch (GridType)
                {
                    case MirGridType.Inventory:
                        return MapObject.User.Inventory;
                    case MirGridType.Equipment:
                        return MapObject.User.Equipment;
                    case MirGridType.BuyBack:
                        //return BuyBackPanel.Goods;
                    case MirGridType.Storage:
                        return GameScene.Storage;
                    case MirGridType.Inspect:
                        return InspectDialog.Items;
                    default:
                        throw new NotImplementedException();
                }

            }
        }

        public override bool Border
        {
            get { return (GameScene.SelectedCell == this || MouseControl == this || Locked) && GridType != MirGridType.DropPanel; }
        }

        private bool _locked;

        public bool Locked
        {
            get { return _locked; }
            set
            {
                if (_locked == value) return;
                _locked = value;
                Redraw();
            }
        }



        #region GridType

        private MirGridType _gridType;
        public event EventHandler GridTypeChanged;
        public MirGridType GridType
        {
            get { return _gridType; }
            set
            {
                if (_gridType == value) return;
                _gridType = value;
                OnGridTypeChanged();
            }
        }

        private void OnGridTypeChanged()
        {
            if (GridTypeChanged != null)
                GridTypeChanged.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region ItemSlot

        private int _itemSlot;
        public event EventHandler ItemSlotChanged;
        public int ItemSlot
        {
            get { return _itemSlot; }
            set
            {
                if (_itemSlot == value) return;
                _itemSlot = value;
                OnItemSlotChanged();
            }
        }

        private void OnItemSlotChanged()
        {
            if (ItemSlotChanged != null)
                ItemSlotChanged.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Count Label

        private MirLabel CountLabel { get; set; }

        #endregion

        public MirItemCell()
        {
            Size = new Size(36, 32);
            GridType = MirGridType.None;
            DrawImage = false;

            BorderColour = Color.Lime;

            BackColour = Color.FromArgb(255, 255, 125, 125);
            Opacity = 0.5F;
            DrawControlTexture = true;
            Library = Libraries.Items;

        }


        public override void OnMouseClick(MouseEventArgs e)
        {
            if (Locked) return;

            if (GameScene.PickedUpGold || GridType == MirGridType.Inspect || GridType == MirGridType.TrustMerchant) return;

            base.OnMouseClick(e);
            
            Redraw();

            switch (e.Button)
            {
                case MouseButtons.Right:
                    UseItem();
                    break;
                case MouseButtons.Left:
                    if (Item != null && GameScene.SelectedCell == null)
                        PlayItemSound();
                    if (CMain.Shift)
                    {
                        if (GridType == MirGridType.Inventory || GridType == MirGridType.Storage)
                        {
                            if (GameScene.SelectedCell == null && Item != null)
                            {
                                if (FreeSpace() == 0)
                                {
                                    GameScene.Scene.ChatDialog.ReceiveChat("No room to split stack.", ChatType.System);
                                    return;
                                }

                                if (Item.Count > 1)
                                {
                                    MirAmountBox amountBox = new MirAmountBox("Split Amount:", Item.Info.Image, Item.Count - 1);

                                    amountBox.OKButton.Click += (o, a) =>
                                    {
                                        if (amountBox.Amount == 0 || amountBox.Amount >= Item.Count) return;
                                        Network.Enqueue(new C.SplitItem { Grid = GridType, UniqueID = Item.UniqueID, Count = amountBox.Amount });
                                        Locked = true;
                                    };

                                    amountBox.Show();
                                }
                            }
                        }
                    }
                    else MoveItem();
                    break;
            }
        }
        public override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (Locked) return;

            if (GameScene.PickedUpGold || GridType == MirGridType.Inspect || GridType == MirGridType.TrustMerchant) return;

            base.OnMouseClick(e);

            Redraw();

            GameScene.SelectedCell = null;
            UseItem();
        }


        private void BuyItem()
        {
            if (Item == null || Item.Price() * GameScene.NPCRate > GameScene.Gold) return;

            MirAmountBox amountBox;
            if (Item.Count > 1)
            {
                amountBox = new MirAmountBox("Purchase Amount:", Item.Info.Image, Item.Count);

                amountBox.OKButton.Click += (o, e) =>
                {
                    Network.Enqueue(new C.BuyItemBack { UniqueID = Item.UniqueID, Count = amountBox.Amount });
                    Locked = true;
                };
            }
            else
            {
                amountBox = new MirAmountBox("Purchase", Item.Info.Image, string.Format("Value: {0:#,##0} Gold", Item.Price()));

                amountBox.OKButton.Click += (o, e) =>
                {
                    Network.Enqueue(new C.BuyItemBack { UniqueID = Item.UniqueID, Count = 1 });
                    Locked = true;
                };
            }

            amountBox.Show();
        }
        public void UseItem()
        {
            if (Locked || GridType == MirGridType.Inspect || GridType == MirGridType.TrustMerchant) return;

            if (GridType == MirGridType.BuyBack)
            {
                BuyItem();
                return;
            }
            if ((GridType != MirGridType.Inventory && GridType != MirGridType.Storage) || Item == null || !CanUseItem() || GameScene.SelectedCell == this) return;


            CharacterDialog dialog = GameScene.Scene.CharacterDialog;


            switch (Item.Info.Type)
            {
                case ItemType.Weapon:
                    if (dialog.Grid[(int)EquipmentSlot.Weapon].CanWearItem(Item))
                    {
                        Network.Enqueue(new C.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.Weapon });
                        dialog.Grid[(int)EquipmentSlot.Weapon].Locked = true;
                        Locked = true;
                    }
                    break;
                case ItemType.Armour:
                    if (dialog.Grid[(int)EquipmentSlot.Armour].CanWearItem(Item))
                    {
                        Network.Enqueue(new C.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.Armour });
                        dialog.Grid[(int)EquipmentSlot.Armour].Locked = true;
                        Locked = true;
                    }
                    break;
                case ItemType.Helmet:
                    if (dialog.Grid[(int)EquipmentSlot.Helmet].CanWearItem(Item))
                    {
                        Network.Enqueue(new C.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.Helmet });
                        dialog.Grid[(int)EquipmentSlot.Helmet].Locked = true;
                        Locked = true;
                    }
                    return;
                case ItemType.Necklace:
                    if (dialog.Grid[(int)EquipmentSlot.Necklace].CanWearItem(Item))
                    {
                        Network.Enqueue(new C.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.Necklace });
                        dialog.Grid[(int)EquipmentSlot.Necklace].Locked = true;
                        Locked = true;
                    }
                    break;
                case ItemType.Bracelet:
                    if (dialog.Grid[(int)EquipmentSlot.BraceletR].Item == null && dialog.Grid[(int)EquipmentSlot.BraceletR].CanWearItem(Item))
                    {
                        Network.Enqueue(new C.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.BraceletR });
                        dialog.Grid[(int)EquipmentSlot.BraceletR].Locked = true;
                        Locked = true;
                    }
                    else if (dialog.Grid[(int)EquipmentSlot.BraceletL].CanWearItem(Item))
                    {
                        Network.Enqueue(new C.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.BraceletL });
                        dialog.Grid[(int)EquipmentSlot.BraceletL].Locked = true;
                        Locked = true;
                    }
                    break;
                case ItemType.Ring:
                    if (dialog.Grid[(int)EquipmentSlot.RingR].Item == null && dialog.Grid[(int)EquipmentSlot.RingR].CanWearItem(Item))
                    {
                        Network.Enqueue(new C.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.RingR });
                        dialog.Grid[(int)EquipmentSlot.RingR].Locked = true;
                        Locked = true;
                    }
                    else if (dialog.Grid[(int)EquipmentSlot.RingL].CanWearItem(Item))
                    {
                        Network.Enqueue(new C.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.RingL });
                        dialog.Grid[(int)EquipmentSlot.RingL].Locked = true;
                        Locked = true;
                    }
                    break;
                case ItemType.Amulet:
                    if (Item.Info.Shape == 0) return;

                    if (dialog.Grid[(int)EquipmentSlot.Amulet].Item != null && Item.Info.Type == ItemType.Amulet)
                    {
                        if (dialog.Grid[(int)EquipmentSlot.Amulet].Item.Info == Item.Info && dialog.Grid[(int)EquipmentSlot.Amulet].Item.Count < dialog.Grid[(int)EquipmentSlot.Amulet].Item.Info.StackSize)
                        {
                            Network.Enqueue(new C.MergeItem { GridFrom = GridType, GridTo = MirGridType.Equipment, IDFrom = Item.UniqueID, IDTo = dialog.Grid[(int)EquipmentSlot.Amulet].Item.UniqueID });

                            Locked = true;
                            GameScene.SelectedCell.Locked = true;
                            GameScene.SelectedCell = null;
                            return;
                        }
                    }

                    if (dialog.Grid[(int)EquipmentSlot.Amulet].CanWearItem(Item))
                    {
                        Network.Enqueue(new C.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.Amulet });
                        dialog.Grid[(int)EquipmentSlot.Amulet].Locked = true;
                        Locked = true;
                    }
                    break;
                case ItemType.Belt:
                    if (dialog.Grid[(int)EquipmentSlot.Belt].CanWearItem(Item))
                    {
                        Network.Enqueue(new C.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.Belt });
                        dialog.Grid[(int)EquipmentSlot.Belt].Locked = true;
                        Locked = true;
                    }
                    break;
                case ItemType.Boots:
                    if (dialog.Grid[(int)EquipmentSlot.Boots].CanWearItem(Item))
                    {
                        Network.Enqueue(new C.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.Boots });
                        dialog.Grid[(int)EquipmentSlot.Boots].Locked = true;
                        Locked = true;
                    }
                    break;
                case ItemType.Stone:
                    if (dialog.Grid[(int)EquipmentSlot.Stone].CanWearItem(Item))
                    {
                        Network.Enqueue(new C.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.Stone });
                        dialog.Grid[(int)EquipmentSlot.Stone].Locked = true;
                        Locked = true;
                    }
                    break;
                case ItemType.Torch:
                    if (dialog.Grid[(int)EquipmentSlot.Torch].CanWearItem(Item))
                    {
                        Network.Enqueue(new C.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.Torch });
                        dialog.Grid[(int)EquipmentSlot.Torch].Locked = true;
                        Locked = true;
                    }
                    break;
                case ItemType.Potion:
                case ItemType.Scroll:
                case ItemType.Book:
                    if (CanUseItem() && GridType == MirGridType.Inventory)
                    {
                        if (CMain.Time < GameScene.UseItemTime) return;
                        Network.Enqueue(new C.UseItem { UniqueID = Item.UniqueID });

                        if (Item.Count == 1 && ItemSlot >= 40)
                        {
                            for (int i = 0; i < 40; i++)
                                if (ItemArray[i] != null && ItemArray[i].Info == Item.Info)
                                {
                                    Network.Enqueue(new C.MoveItem { Grid = MirGridType.Inventory, From = i, To = ItemSlot });
                                    GameScene.Scene.InventoryDialog.Grid[i].Locked = true;
                                    break;
                                }
                        }

                        Locked = true;
                    }
                    break;
                case ItemType.Tiger:
                    break;
            }

            GameScene.UseItemTime = CMain.Time + 300;
            PlayItemSound();
        }
        private void MoveItem()
        {
            if (GridType == MirGridType.BuyBack || GridType == MirGridType.DropPanel || GridType == MirGridType.Inspect || GridType == MirGridType.TrustMerchant) return;

            if (GameScene.SelectedCell == this)
            {
                GameScene.SelectedCell = null;
                return;
            }

            if (GameScene.SelectedCell != null)
            {
                if (GameScene.SelectedCell.Item == null)
                {
                    GameScene.SelectedCell = null;
                    return;
                }

                switch (GridType)
                {
                    #region To Inventory
                    case MirGridType.Inventory: // To Inventory
                        switch (GameScene.SelectedCell.GridType)
                        {
                            #region From Inventory
                            case MirGridType.Inventory: //From Invenotry
                                if (Item != null)
                                {
                                    if (GameScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                                    {
                                        //Merge.
                                        Network.Enqueue(new C.MergeItem { GridFrom = GameScene.SelectedCell.GridType, GridTo = GridType, IDFrom = GameScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                        Locked = true;
                                        GameScene.SelectedCell.Locked = true;
                                        GameScene.SelectedCell = null;
                                        return;
                                    }
                                }

                                Network.Enqueue(new C.MoveItem { Grid = GridType, From = GameScene.SelectedCell.ItemSlot, To = ItemSlot });

                                Locked = true;
                                GameScene.SelectedCell.Locked = true;
                                GameScene.SelectedCell = null;
                                return;
                            #endregion
                            #region From Equipment
                            case MirGridType.Equipment: //From Equipment
                                if (Item != null && GameScene.SelectedCell.Item.Info.Type == ItemType.Amulet)
                                {
                                    if (GameScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                                    {
                                        Network.Enqueue(new C.MergeItem { GridFrom = GameScene.SelectedCell.GridType, GridTo = GridType, IDFrom = GameScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                        Locked = true;
                                        GameScene.SelectedCell.Locked = true;
                                        GameScene.SelectedCell = null;
                                        return;
                                    }
                                }

                                if (!CanRemoveItem(GameScene.SelectedCell.Item))
                                {
                                    GameScene.SelectedCell = null;
                                    return;
                                }


                                if (Item == null)
                                {
                                    Network.Enqueue(new C.RemoveItem { Grid = GridType, UniqueID = GameScene.SelectedCell.Item.UniqueID, To = ItemSlot });

                                    Locked = true;
                                    GameScene.SelectedCell.Locked = true;
                                    GameScene.SelectedCell = null;
                                    return;
                                }

                                for (int x = 0; x < ItemArray.Length; x++)
                                    if (ItemArray[x] == null)
                                    {
                                        Network.Enqueue(new C.RemoveItem { Grid = GridType, UniqueID = GameScene.SelectedCell.Item.UniqueID, To = x });

                                        MirItemCell temp = x < 40 ? GameScene.Scene.InventoryDialog.Grid[x] : GameScene.Scene.BeltDialog.Grid[x - 40];

                                        if (temp != null) temp.Locked = true;
                                        GameScene.SelectedCell.Locked = true;
                                        GameScene.SelectedCell = null;
                                        return;
                                    }
                                break;
                            #endregion
                            #region From Storage
                            case MirGridType.Storage: //From Storage
                                if (Item != null && GameScene.SelectedCell.Item.Info.Type == ItemType.Amulet)
                                {
                                    if (GameScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                                    {
                                        Network.Enqueue(new C.MergeItem { GridFrom = GameScene.SelectedCell.GridType, GridTo = GridType, IDFrom = GameScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                        Locked = true;
                                        GameScene.SelectedCell.Locked = true;
                                        GameScene.SelectedCell = null;
                                        return;
                                    }
                                }

                                if (GameScene.SelectedCell.Item.Weight + MapObject.User.CurrentBagWeight > MapObject.User.MaxBagWeight)
                                {
                                    GameScene.Scene.ChatDialog.ReceiveChat("Too heavy to get back.", ChatType.System);
                                    GameScene.SelectedCell = null;
                                    return;
                                }

                                if (Item != null)
                                {
                                    if (GameScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                                    {
                                        Network.Enqueue(new C.MergeItem { GridFrom = GameScene.SelectedCell.GridType, GridTo = GridType, IDFrom = GameScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                        Locked = true;
                                        GameScene.SelectedCell.Locked = true;
                                        GameScene.SelectedCell = null;
                                        return;
                                    }
                                }


                                if (Item == null)
                                {
                                    Network.Enqueue(new C.TakeBackItem { From = GameScene.SelectedCell.ItemSlot, To = ItemSlot });

                                    Locked = true;
                                    GameScene.SelectedCell.Locked = true;
                                    GameScene.SelectedCell = null;
                                    return;
                                }

                                for (int x = 0; x < ItemArray.Length; x++)
                                    if (ItemArray[x] == null)
                                    {
                                        Network.Enqueue(new C.TakeBackItem { From = GameScene.SelectedCell.ItemSlot, To = x });

                                        MirItemCell temp = x < 40 ? GameScene.Scene.InventoryDialog.Grid[x] : GameScene.Scene.BeltDialog.Grid[x - 40];
                                       // MirItemCell Temp = GameScene.Scene.BagDialog.Grid.FirstOrDefault(A => A.ItemSlot == X) ??
                                        //                   GameScene.Scene.BeltDialog.Grid.FirstOrDefault(A => A.ItemSlot == X);

                                        if (temp != null) temp.Locked = true;
                                        GameScene.SelectedCell.Locked = true;
                                        GameScene.SelectedCell = null;
                                        return;
                                    }
                                break;
                            #endregion

                        }
                        break;
                    #endregion
                    #region To Equipment
                    case MirGridType.Equipment: //To Equipment

                        if (GameScene.SelectedCell.GridType != MirGridType.Inventory && GameScene.SelectedCell.GridType != MirGridType.Storage) return;


                        if (Item != null && GameScene.SelectedCell.Item.Info.Type == ItemType.Amulet)
                        {
                            if (GameScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                            {
                                Network.Enqueue(new C.MergeItem { GridFrom = GameScene.SelectedCell.GridType, GridTo = GridType, IDFrom = GameScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                Locked = true;
                                GameScene.SelectedCell.Locked = true;
                                GameScene.SelectedCell = null;
                                return;
                            }
                        }

                        if (CorrectSlot(GameScene.SelectedCell.Item))
                        {
                            if (CanWearItem(GameScene.SelectedCell.Item))
                            {
                                Network.Enqueue(new C.EquipItem { Grid = GameScene.SelectedCell.GridType, UniqueID = GameScene.SelectedCell.Item.UniqueID, To = ItemSlot });
                                Locked = true;
                                GameScene.SelectedCell.Locked = true;
                            }
                            GameScene.SelectedCell = null;
                        }
                        return;
                    #endregion
                    #region To Storage
                    case MirGridType.Storage: //To Storage
                        switch (GameScene.SelectedCell.GridType)
                        {
                            #region From Inventory
                            case MirGridType.Inventory: //From Invenotry
                                if (Item != null)
                                {
                                    if (GameScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                                    {
                                        Network.Enqueue(new C.MergeItem { GridFrom = GameScene.SelectedCell.GridType, GridTo = GridType, IDFrom = GameScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                        Locked = true;
                                        GameScene.SelectedCell.Locked = true;
                                        GameScene.SelectedCell = null;
                                        return;
                                    }
                                }


                                if (ItemArray[ItemSlot] == null)
                                {
                                    Network.Enqueue(new C.StoreItem { From = GameScene.SelectedCell.ItemSlot, To = ItemSlot });
                                    Locked = true;
                                    GameScene.SelectedCell.Locked = true;
                                    GameScene.SelectedCell = null;
                                    return;
                                }

                                for (int x = 0; x < ItemArray.Length; x++)
                                    if (ItemArray[x] == null)
                                    {
                                        Network.Enqueue(new C.StoreItem { From = GameScene.SelectedCell.ItemSlot, To = x });

                                        MirItemCell temp = GameScene.Scene.StorageDialog.Grid[x];
                                        if (temp != null) temp.Locked = true;
                                        GameScene.SelectedCell.Locked = true;
                                        GameScene.SelectedCell = null;
                                        return;
                                    }
                                break;
                            #endregion
                            #region From Equipment
                            case MirGridType.Equipment: //From Equipment
                                if (Item != null)
                                {
                                    if (GameScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                                    {
                                        //Merge.
                                        Network.Enqueue(new C.MergeItem { GridFrom = GameScene.SelectedCell.GridType, GridTo = GridType, IDFrom = GameScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                        Locked = true;
                                        GameScene.SelectedCell.Locked = true;
                                        GameScene.SelectedCell = null;
                                        return;
                                    }
                                }

                                if (!CanRemoveItem(GameScene.SelectedCell.Item))
                                {
                                    GameScene.SelectedCell = null;
                                    return;
                                }

                                if (Item == null)
                                {
                                    Network.Enqueue(new C.RemoveItem { Grid = GridType, UniqueID = GameScene.SelectedCell.Item.UniqueID, To = ItemSlot });

                                    Locked = true;
                                    GameScene.SelectedCell.Locked = true;
                                    GameScene.SelectedCell = null;
                                    return;
                                }

                                for (int x = 0; x < ItemArray.Length; x++)
                                    if (ItemArray[x] == null)
                                    {
                                        Network.Enqueue(new C.RemoveItem { Grid = GridType, UniqueID = GameScene.SelectedCell.Item.UniqueID, To = x });

                                        MirItemCell temp = GameScene.Scene.StorageDialog.Grid[x];
                                        if (temp != null) temp.Locked = true;
                                        GameScene.SelectedCell.Locked = true;
                                        GameScene.SelectedCell = null;
                                        return;
                                    }
                                break;
                            #endregion
                            #region From Storage
                            case MirGridType.Storage:
                                if (Item != null)
                                {
                                    if (GameScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                                    {
                                        //Merge.
                                        Network.Enqueue(new C.MergeItem { GridFrom = GameScene.SelectedCell.GridType, GridTo = GridType, IDFrom = GameScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                        Locked = true;
                                        GameScene.SelectedCell.Locked = true;
                                        GameScene.SelectedCell = null;
                                        return;
                                    }
                                }

                                Network.Enqueue(new C.MoveItem { Grid = GridType, From = GameScene.SelectedCell.ItemSlot, To = ItemSlot });
                                Locked = true;
                                GameScene.SelectedCell.Locked = true;
                                GameScene.SelectedCell = null;
                                return;
                            #endregion
                        }
                        break;

                    #endregion
                }

                return;
            }

            if (Item != null)
                GameScene.SelectedCell = this;
        }
        private void PlayItemSound()
        {
            if (Item == null) return;

            switch (Item.Info.Type)
            {
                case ItemType.Weapon:
                    SoundManager.PlaySound(SoundList.ClickWeapon);
                    break;
                case ItemType.Armour:
                    SoundManager.PlaySound(SoundList.ClickArmour);
                    break;
                case ItemType.Helmet:
                    SoundManager.PlaySound(SoundList.ClickHelemt);
                    break;
                case ItemType.Necklace:
                    SoundManager.PlaySound(SoundList.ClickNecklace);
                    break;
                case ItemType.Bracelet:
                    SoundManager.PlaySound(SoundList.ClickBracelet);
                    break;
                case ItemType.Ring:
                    SoundManager.PlaySound(SoundList.ClickRing);
                    break;
                case ItemType.Boots:
                    SoundManager.PlaySound(SoundList.ClickBoots);
                    break;
                case ItemType.Potion:
                    SoundManager.PlaySound(SoundList.ClickDrug);
                    break;
                default:
                    SoundManager.PlaySound(SoundList.ClickItem);
                    break;
            }
        }
        private int FreeSpace()
        {
            int count = 0;

            for (int i = 0; i < ItemArray.Length; i++)
                if (ItemArray[i] == null) count++;

            return count;
        }


        private bool CanRemoveItem(UserItem i)
        {
            //stuck
            return FreeSpace() > 0;
        }

        private bool CorrectSlot(UserItem i)
        {
            ItemType type = i.Info.Type;

            switch ((EquipmentSlot)ItemSlot)
            {
                case EquipmentSlot.Weapon:
                    return type == ItemType.Weapon;
                case EquipmentSlot.Armour:
                    return type == ItemType.Armour;
                case EquipmentSlot.Helmet:
                    return type == ItemType.Helmet;
                case EquipmentSlot.Torch:
                    return type == ItemType.Torch;
                case EquipmentSlot.Necklace:
                    return type == ItemType.Necklace;
                case EquipmentSlot.BraceletL:
                    return i.Info.Type == ItemType.Bracelet;
                case EquipmentSlot.BraceletR:
                    return i.Info.Type == ItemType.Bracelet;
                case EquipmentSlot.RingL:
                case EquipmentSlot.RingR:
                    return type == ItemType.Ring;
                case EquipmentSlot.Amulet:
                    return type == ItemType.Amulet && i.Info.Shape > 0;
                case EquipmentSlot.Boots:
                    return type == ItemType.Boots;
                case EquipmentSlot.Belt:
                    return type == ItemType.Belt;
                case EquipmentSlot.Stone:
                    return type == ItemType.Stone;
                default:
                    return false;
            }

        }
        private bool CanUseItem()
        {
            if (Item == null) return false;


            switch (MapObject.User.Gender)
            {
                case MirGender.Male:
                    if (!Item.Info.RequiredGender.HasFlag(RequiredGender.Male))
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("You are not Female.", ChatType.System);
                        return false;
                    }
                    break;
                case MirGender.Female:
                    if (!Item.Info.RequiredGender.HasFlag(RequiredGender.Female))
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("You are not Male.", ChatType.System);
                        return false;
                    }
                    break;
            }

            switch (MapObject.User.Class)
            {
                case MirClass.Warrior:
                    if (!Item.Info.RequiredClass.HasFlag(RequiredClass.Warrior))
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("Warriors cannot use this item.", ChatType.System);
                        return false;
                    }
                    break;
                case MirClass.Wizard:
                    if (!Item.Info.RequiredClass.HasFlag(RequiredClass.Wizard))
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("Wizards cannot use this item.", ChatType.System);
                        return false;
                    }
                    break;
                case MirClass.Taoist:
                    if (!Item.Info.RequiredClass.HasFlag(RequiredClass.Taoist))
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("Taoists cannot use this item.", ChatType.System);
                        return false;
                    }
                    break;
                case MirClass.Assassin:
                    if (!Item.Info.RequiredClass.HasFlag(RequiredClass.Assassin))
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("Assassins cannot use this item.", ChatType.System);
                        return false;
                    }
                    break;
            }

            switch (Item.Info.RequiredType)
            {
                case RequiredType.Level:
                    if (MapObject.User.Level < Item.Info.RequiredAmount)
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("You are not a high enough level.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.AC:
                    if (MapObject.User.MaxAC < Item.Info.RequiredAmount)
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("You do not have enough AC.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MAC:
                    if (MapObject.User.MaxMAC < Item.Info.RequiredAmount)
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("You do not have enough MAC.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.DC:
                    if (MapObject.User.MaxDC < Item.Info.RequiredAmount)
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("You do not have enough DC.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MC:
                    if (MapObject.User.MaxMC < Item.Info.RequiredAmount)
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("You do not have enough MC.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.SC:
                    if (MapObject.User.MaxSC < Item.Info.RequiredAmount)
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("You do not have enough SC.", ChatType.System);
                        return false;
                    }
                    break;
            }
            return true;
        }

        private bool CanWearItem(UserItem i)
        {
            if (i == null) return false;

            //If Can remove;

            switch (MapObject.User.Gender)
            {
                case MirGender.Male:
                    if (!i.Info.RequiredGender.HasFlag(RequiredGender.Male))
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("You are not Female.", ChatType.System);
                        return false;
                    }
                    break;
                case MirGender.Female:
                    if (!i.Info.RequiredGender.HasFlag(RequiredGender.Female))
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("You are not Male.", ChatType.System);
                        return false;
                    }
                    break;
            }

            switch (MapObject.User.Class)
            {
                case MirClass.Warrior:
                    if (!i.Info.RequiredClass.HasFlag(RequiredClass.Warrior))
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("Warriors cannot use this item.", ChatType.System);
                        return false;
                    }
                    break;
                case MirClass.Wizard:
                    if (!i.Info.RequiredClass.HasFlag(RequiredClass.Wizard))
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("Wizards cannot use this item.", ChatType.System);
                        return false;
                    }
                    break;
                case MirClass.Taoist:
                    if (!i.Info.RequiredClass.HasFlag(RequiredClass.Taoist))
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("Taoists cannot use this item.", ChatType.System);
                        return false;
                    }
                    break;
                case MirClass.Assassin:
                    if (!i.Info.RequiredClass.HasFlag(RequiredClass.Assassin))
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("Assassins cannot use this item.", ChatType.System);
                        return false;
                    }
                    break;
            }

            switch (i.Info.RequiredType)
            {
                case RequiredType.Level:
                    if (MapObject.User.Level < i.Info.RequiredAmount)
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("You are not a high enough level.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.AC:
                    if (MapObject.User.MaxAC < i.Info.RequiredAmount)
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("You do not have enough AC.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MAC:
                    if (MapObject.User.MaxMAC < i.Info.RequiredAmount)
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("You do not have enough MAC.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.DC:
                    if (MapObject.User.MaxDC < i.Info.RequiredAmount)
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("You do not have enough DC.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MC:
                    if (MapObject.User.MaxMC < i.Info.RequiredAmount)
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("You do not have enough MC.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.SC:
                    if (MapObject.User.MaxSC < i.Info.RequiredAmount)
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("You do not have enough SC.", ChatType.System);
                        return false;
                    }
                    break;
            }

            if (i.Info.Type == ItemType.Weapon || i.Info.Type == ItemType.Torch)
            {
                if (i.Weight - (Item != null ? Item.Weight : 0) + MapObject.User.CurrentHandWeight > MapObject.User.MaxHandWeight)
                {
                    GameScene.Scene.ChatDialog.ReceiveChat("It is too heavy to Hold.", ChatType.System);
                    return false;
                }
            }
            else
            {
                if (i.Weight - (Item != null ? Item.Weight : 0) + MapObject.User.CurrentWearWeight > MapObject.User.MaxWearWeight)
                {
                    GameScene.Scene.ChatDialog.ReceiveChat("It is too heavy to wear.", ChatType.System);
                    return false;
                }
            }

            return true;
        }

        protected internal override void DrawControl()
        {
            if (GameScene.SelectedCell == this || Locked)
                base.DrawControl();

            if (Locked) return;

            if (Item != null && GameScene.SelectedCell != this)
            {
                CreateDisposeLabel();

                if (Library != null)
                {
                    Size imgSize = Library.GetTrueSize(Item.Info.Image);
                    Point offSet = new Point((Size.Width - imgSize.Width) / 2, (Size.Height - imgSize.Height) / 2);
                    Library.Draw(Item.Info.Image, DisplayLocation.Add(offSet), ForeColour, UseOffSet, 1F);
                }

            }
            else
                DisposeCountLabel();
        }

        protected override void OnMouseEnter()
        {
            base.OnMouseEnter();
            GameScene.Scene.CreateItemLabel(Item);
        }
        protected override void OnMouseLeave()
        {
            base.OnMouseLeave();
            GameScene.Scene.DisposeItemLabel();
            GameScene.HoverItem = null;
        }

        private void CreateDisposeLabel()
        {
            if (Item.Info.StackSize <= 1)
            {
                DisposeCountLabel();
                return;
            }

            if (CountLabel == null || CountLabel.IsDisposed)
            {
                CountLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    NotControl = true,
                    OutLine = false,
                    Parent = this,
                };
            }

            CountLabel.Text = Item.Count.ToString("###0");
            CountLabel.Location = new Point(Size.Width - CountLabel.Size.Width, Size.Height - CountLabel.Size.Height);
        }
        private void DisposeCountLabel()
        {
            if (CountLabel != null && !CountLabel.IsDisposed)
                CountLabel.Dispose();
            CountLabel = null;
        }
    }
}
