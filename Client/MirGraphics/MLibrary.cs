using System;
using System.Drawing;
using System.IO;
using System.Threading;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Client.MirGraphics
{
    public static class Libraries
    {
        public static bool Loaded;
        public static int Count, Progress;


        public static readonly MLibrary
            ChrSel = new MLibrary(Settings.DataPath + "ChrSel"),
            Prguse = new MLibrary(Settings.DataPath + "Prguse"),
            Prguse2 = new MLibrary(Settings.DataPath + "Prguse2"),
            MiniMap = new MLibrary(Settings.DataPath + "MMap"),
            Title = new MLibrary(Settings.DataPath + "Title"),
            MagIcon = new MLibrary(Settings.DataPath + "MagIcon"),
            MagIcon2 = new MLibrary(Settings.DataPath + "MagIcon2"),
            Magic = new MLibrary(Settings.DataPath + "Magic"),
            Magic2 = new MLibrary(Settings.DataPath + "Magic2"),
            Magic3 = new MLibrary(Settings.DataPath + "Magic3");

        public static readonly MLibrary
            Dragon = new MLibrary(Settings.DataPath + "Dragon");

        //Map
        public static readonly MLibrary
            Tiles = new MLibrary(Settings.DataPath + "Tiles"),
            SmallTiles = new MLibrary(Settings.DataPath + "SmTiles");

        //Items
        public static readonly MLibrary
            Items = new MLibrary(Settings.DataPath + "Items"),
            StateItems = new MLibrary(Settings.DataPath + "StateItem"),
            FloorItems = new MLibrary(Settings.DataPath + "DNItems");


        public static readonly MLibrary[] CArmours = new MLibrary[35],
                                          CWeapons = new MLibrary[51],
                                          CHair = new MLibrary[8],
                                          AArmours = new MLibrary[12],
                                          AWeaponsL = new MLibrary[11],
                                          AWeaponsR = new MLibrary[11],
                                          AHair = new MLibrary[9],
                                          Monsters = new MLibrary[139],
                                          NPCs = new MLibrary[42],
                                          CHumEffect = new MLibrary[2];

        public static readonly MLibrary[] Objects = {
            new MLibrary(Settings.DataPath +"Objects"),
            new MLibrary(Settings.DataPath +"Objects2"),
            new MLibrary(Settings.DataPath +"Objects3"),
            new MLibrary(Settings.DataPath +"Objects4"),
            new MLibrary(Settings.DataPath +"Objects5"),
            new MLibrary(Settings.DataPath +"Objects6"),
            new MLibrary(Settings.DataPath +"Objects7"),
            new MLibrary(Settings.DataPath +"Objects8"),
            new MLibrary(Settings.DataPath +"Objects9"),
            new MLibrary(Settings.DataPath +"Objects10"),
            new MLibrary(Settings.DataPath +"Objects11"),
            new MLibrary(Settings.DataPath +"Objects12"),
            new MLibrary(Settings.DataPath +"Objects13"),
            new MLibrary(Settings.DataPath +"Objects14"),
            new MLibrary(Settings.DataPath +"Objects15"),
            new MLibrary(Settings.DataPath +"Objects16"),
            new MLibrary(Settings.DataPath +"Objects17"),
            new MLibrary(Settings.DataPath +"Objects18"),
            new MLibrary(Settings.DataPath +"Objects19"),
            new MLibrary(Settings.DataPath +"Objects20")
        };


        static Libraries()
        {
            for (int i = 0; i < CArmours.Length; i++)
                CArmours[i] = new MLibrary(Settings.CArmourPath + i.ToString("00"));

            for (int i = 0; i < CHair.Length; i++)
                CHair[i] = new MLibrary(Settings.CHairPath + i.ToString("00"));

            for (int i = 0; i < CWeapons.Length; i++)
                CWeapons[i] = new MLibrary(Settings.CWeaponPath + i.ToString("00"));

            for (int i = 0; i < AArmours.Length; i++)
                AArmours[i] = new MLibrary(Settings.AArmourPath + i.ToString("00"));

            for (int i = 0; i < AHair.Length; i++)
                AHair[i] = new MLibrary(Settings.AHairPath + i.ToString("00"));

            for (int i = 0; i < AWeaponsL.Length; i++)
                AWeaponsL[i] = new MLibrary(Settings.AWeaponPath + i.ToString("00") + " L");

            for (int i = 0; i < AWeaponsR.Length; i++)
                AWeaponsR[i] = new MLibrary(Settings.AWeaponPath + i.ToString("00") + " R");

            for (int i = 0; i < Monsters.Length; i++)
                Monsters[i] = new MLibrary(Settings.MonsterPath + i.ToString("000"));

            for (int i = 0; i < NPCs.Length; i++)
                NPCs[i] = new MLibrary(Settings.NPCPath + i.ToString("00"));

            for (int i = 0; i < CHumEffect.Length; i++)
                CHumEffect[i] = new MLibrary(Settings.CHumEffectPath + i.ToString("00"));
            Thread thread = new Thread(LoadLibraries) { IsBackground = true };
            thread.Start();
        }

        static void LoadLibraries()
        {
            Count = Objects.Length + Monsters.Length + NPCs.Length + CArmours.Length + 
                CHair.Length + CWeapons.Length + AArmours.Length + AHair.Length + AWeaponsL.Length + AWeaponsR.Length +
                CHumEffect.Length + 13;

            Tiles.Initialize();
            Progress++;
            SmallTiles.Initialize();
            Progress++;

            Dragon.Initialize();
            Progress++;

            Prguse2.Initialize();
            Progress++;
            MiniMap.Initialize();
            Progress++;
            MagIcon.Initialize();
            Progress++;
            MagIcon2.Initialize();
            Progress++;

            Magic.Initialize();
            Progress++;
            Magic2.Initialize();
            Progress++;
            Magic3.Initialize();
            Progress++;

            Items.Initialize();
            Progress++;
            StateItems.Initialize();
            Progress++;
            FloorItems.Initialize();
            Progress++;

            for (int i = 0; i < Objects.Length; i++)
            {
                Objects[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < Monsters.Length; i++)
            {
                Monsters[i].Initialize();
                Progress++;
            }


            for (int i = 0; i < NPCs.Length; i++)
            {
                NPCs[i].Initialize();
                Progress++;
            }


            for (int i = 0; i < CArmours.Length; i++)
            {
                CArmours[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < CHair.Length; i++)
            {
                CHair[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < CWeapons.Length; i++)
            {
                CWeapons[i].Initialize();
                Progress++;
            }


            for (int i = 0; i < AArmours.Length; i++)
            {
                AArmours[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < AHair.Length; i++)
            {
                AHair[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < AWeaponsL.Length; i++)
            {
                AWeaponsL[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < AWeaponsR.Length; i++)
            {
               AWeaponsR[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < CHumEffect.Length; i++)
            {
                CHumEffect[i].Initialize();
                Progress++;
            }

            Loaded = true;
        }

    }
    
    public sealed class MLibrary
    {
        private const string Extention = ".Lib";
        public const int LibVersion = 1;

        private readonly string _fileName;

        private MImage[] _images;
        private int[] _indexList;
        private int _count;
        private bool _initialized;

        private BinaryReader _reader;
        private FileStream _fStream;

        public MLibrary(string filename)
        {
            _fileName = Path.ChangeExtension(filename, Extention);
        }
        
        public void Initialize()
        {
            int CurrentVersion = 0;
            _initialized = true;

            if (!File.Exists(_fileName))
                return;
            try
            {

                _fStream = new FileStream(_fileName, FileMode.Open, FileAccess.Read);
                _reader = new BinaryReader(_fStream);
                CurrentVersion = _reader.ReadInt32();
                if (CurrentVersion != LibVersion)
                {//cant use a directx based error popup cause it could be the lib file containing the interface is invalid :(
                    System.Windows.Forms.MessageBox.Show("Wrong version, expecting lib version: " + LibVersion.ToString() + " found version: " + CurrentVersion.ToString() + ".", _fileName, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error, System.Windows.Forms.MessageBoxDefaultButton.Button1);
                    System.Windows.Forms.Application.Exit();
                    return;
                }
                _count = _reader.ReadInt32();
                _images = new MImage[_count];
                _indexList = new int[_count];

                for (int i = 0; i < _count; i++)
                    _indexList[i] = _reader.ReadInt32();
            }
            catch (Exception)
            {
                _initialized = false;
                throw;
            }
        }

        private bool CheckImage(int index)
        {
            if (!_initialized)
                Initialize();

            if (_images == null || index < 0 || index >= _images.Length)
                return false;


            if (_images[index] == null)
            {
                _fStream.Position = _indexList[index];
                _images[index] = new MImage(_reader);
            }

            MImage mi = _images[index];
            if (!mi.TextureValid)
            {
                _fStream.Seek(_indexList[index] + 17, SeekOrigin.Begin);
                mi.CreateTexture(_reader);
            }

            return true;
        }

        public Point GetOffSet(int index)
        {
            if (!_initialized) Initialize();

            if (_images == null || index < 0 || index >= _images.Length)
                return Point.Empty;

            if (_images[index] == null)
            {
                _fStream.Seek(_indexList[index], SeekOrigin.Begin);
                _images[index] = new MImage(_reader);
            }

            return new Point(_images[index].X, _images[index].Y);
        }
        public Size GetSize(int index)
        {
            if (!_initialized) Initialize();
            if (_images == null || index < 0 || index >= _images.Length)
                return Size.Empty;

            if (_images[index] == null)
            {
                _fStream.Seek(_indexList[index], SeekOrigin.Begin);
                _images[index] = new MImage(_reader);
            }

            return new Size(_images[index].Width, _images[index].Height);
        }
        public Size GetTrueSize(int index)
        {
            if (!_initialized)
                Initialize();

            if (_images == null || index < 0 || index >= _images.Length)
                return Size.Empty;


            if (_images[index] == null)
            {
                _fStream.Position = _indexList[index];
                _images[index] = new MImage(_reader);
            }
            MImage mi = _images[index];
            if (mi.TrueSize.IsEmpty)
            {
                if (!mi.TextureValid)
                {
                    _fStream.Seek(_indexList[index] + 17, SeekOrigin.Begin);
                    mi.CreateTexture(_reader);
                }
                return mi.GetTrueSize();
            }
            return mi.TrueSize;
        }

        public void Draw(int index, int x, int y)
        {
            if (x >= Settings.ScreenWidth || y >= Settings.ScreenHeight)
                return;

            if (!CheckImage(index))
                return;

            MImage mi = _images[index];

            if (x + mi.Width < 0 || y + mi.Height < 0)
                return;


            DXManager.Sprite.Draw2D(mi.Image, Point.Empty, 0, new PointF(x, y), Color.White);
            mi.CleanTime = CMain.Time + Settings.CleanDelay;
        }
        public void Draw(int index, Point point, Color colour, bool offSet = false)
        {
            if (!CheckImage(index))
                return;

            MImage mi = _images[index];

            if (offSet) point.Offset(mi.X, mi.Y);

            if (point.X >= Settings.ScreenWidth || point.Y >= Settings.ScreenHeight || point.X + mi.Width < 0 || point.Y + mi.Height < 0)
                return;


            
            DXManager.Sprite.Draw2D(mi.Image, Point.Empty, 0, point, colour);

            mi.CleanTime = CMain.Time + Settings.CleanDelay;
        }
        public void Draw(int index, Point point, Color colour, bool offSet, float opacity)
        {
            if (!CheckImage(index))
                return;

            MImage mi = _images[index];

            if (offSet) point.Offset(mi.X, mi.Y);


            if (point.X >= Settings.ScreenWidth || point.Y >= Settings.ScreenHeight || point.X + mi.Width < 0 || point.Y + mi.Height < 0)
                return;

            float oldOpacity = DXManager.Opacity;
            DXManager.SetOpacity(opacity);
            
            DXManager.Sprite.Draw2D(mi.Image, Point.Empty, 0, point, colour);
            DXManager.SetOpacity(oldOpacity);
            mi.CleanTime = CMain.Time + Settings.CleanDelay;
        }
        public void DrawBlend(int index, Point point, Color colour, bool offSet = false, float rate = 1)
        {
            if (!CheckImage(index))
                return;

            MImage mi = _images[index];

            if (offSet) point.Offset(mi.X, mi.Y);


            if (point.X >= Settings.ScreenWidth || point.Y >= Settings.ScreenHeight || point.X + mi.Width < 0 || point.Y + mi.Height < 0)
                return;
            
            bool oldBlend = DXManager.Blending;
            DXManager.SetBlend(true, rate);

            DXManager.Sprite.Draw2D(mi.Image, Point.Empty, 0, point, colour);
 
            DXManager.SetBlend(oldBlend);
            mi.CleanTime = CMain.Time + Settings.CleanDelay;
        }
        public void Draw(int index, Rectangle section, Point point, Color colour, bool offSet)
        {
            if (!CheckImage(index))
                return;

            MImage mi = _images[index];

            if (offSet) point.Offset(mi.X, mi.Y);


            if (point.X >= Settings.ScreenWidth || point.Y >= Settings.ScreenHeight || point.X + mi.Width < 0 || point.Y + mi.Height < 0)
                return;

            if (section.Right > mi.Width)
                section.Width -= section.Right - mi.Width;

            if (section.Bottom > mi.Height)
                section.Height -= section.Bottom - mi.Height;

            DXManager.Sprite.Draw2D(mi.Image, section, section.Size, point, colour);
            mi.CleanTime = CMain.Time + Settings.CleanDelay;
        }
        public void Draw(int index, Rectangle section, Point point, Color colour, float opacity)
        {
            if (!CheckImage(index))
                return;

            MImage mi = _images[index];


            if (point.X >= Settings.ScreenWidth || point.Y >= Settings.ScreenHeight || point.X + mi.Width < 0 || point.Y + mi.Height < 0)
                return;

            if (section.Right > mi.Width)
                section.Width -= section.Right - mi.Width;

            if (section.Bottom > mi.Height)
                section.Height -= section.Bottom - mi.Height;

            float oldOpacity = DXManager.Opacity;
            DXManager.SetOpacity(opacity);

            DXManager.Sprite.Draw2D(mi.Image, section, section.Size, point, colour);

            DXManager.SetOpacity(oldOpacity);
            mi.CleanTime = CMain.Time + Settings.CleanDelay;
        }
        public void Draw(int index, Point point, Size size, Color colour)
        {
            if (!CheckImage(index))
                return;

            MImage mi = _images[index];

            if (point.X >= Settings.ScreenWidth || point.Y >= Settings.ScreenHeight || point.X + size.Width < 0 || point.Y + size.Height < 0)
                return;
            
            DXManager.Sprite.Draw2D(mi.Image, new Rectangle(Point.Empty, new Size(mi.Width,mi.Height)), size, point, colour);
            mi.CleanTime = CMain.Time + Settings.CleanDelay;
        }

        public void DrawTinted(int index, Point point, Color colour, Color Tint)
        {
            if (!CheckImage(index))
                return;

            MImage mi = _images[index];

            if (point.X >= Settings.ScreenWidth || point.Y >= Settings.ScreenHeight || point.X + mi.Width < 0 || point.Y + mi.Height < 0)
                return;
            DXManager.Sprite.Draw2D(mi.Image, Point.Empty, 0, point, colour);

            if (mi.HasMask != true) return;
            DXManager.Sprite.Draw2D(mi.MaskImage, Point.Empty, 0, point, Tint);

            mi.CleanTime = CMain.Time + Settings.CleanDelay;
        }

        
        public bool VisiblePixel(int index, Point point, bool accuate)
        {
            return CheckImage(index) && _images[index].VisiblePixel(point, accuate);
        }

    }
     
    public sealed class MImage
    {
        public short Width, Height, X, Y, ShadowX, ShadowY;
        public byte Shadow;
        public int Length;

        public bool TextureValid;
        public Texture Image;
        //layer 2:
        public short MaskWidth, MaskHeight, MaskX, MaskY;
        public int MaskLength;

        public Texture MaskImage;
        public Boolean HasMask;
       
        public long CleanTime;
        public Size TrueSize;

        public unsafe byte* Data;


        public MImage(BinaryReader reader)
        {

            //read layer 1
            Width = reader.ReadInt16();
            Height = reader.ReadInt16();
            X = reader.ReadInt16();
            Y = reader.ReadInt16();
            ShadowX = reader.ReadInt16();
            ShadowY = reader.ReadInt16();
            Shadow = reader.ReadByte();
            Length = reader.ReadInt32();
            
            //check if there's a second layer and read it
            HasMask = ((Shadow >> 7) == 1) ? true : false;
            if (HasMask)
            {
                reader.ReadBytes(Length);
                MaskWidth = reader.ReadInt16();
                MaskHeight = reader.ReadInt16();
                MaskX = reader.ReadInt16();
                MaskY = reader.ReadInt16();
                MaskLength = reader.ReadInt32();
            }
        }

        public unsafe void CreateTexture(BinaryReader reader)
        {

            int w = Width + (4 - Width % 4) % 4;
            int h = Height + (4 - Height % 4) % 4;

            Image = new Texture(DXManager.Device, w, h, 1, Usage.None, Format.Dxt1, Pool.Managed);
            GraphicsStream stream = Image.LockRectangle(0, LockFlags.Discard);
            Data = (byte*)stream.InternalDataPointer;

            stream.Write(reader.ReadBytes(Length), 0, Length);
            
            stream.Dispose();
            Image.UnlockRectangle(0);

            if (HasMask)
            {
                reader.ReadBytes(12);
                w = Width + (4 - Width % 4) % 4;
                h = Height + (4 - Height % 4) % 4;

                MaskImage = new Texture(DXManager.Device, w, h, 1, Usage.None, Format.Dxt1, Pool.Managed);
                stream = MaskImage.LockRectangle(0, LockFlags.Discard);

                stream.Write(reader.ReadBytes(Length), 0, Length);

                stream.Dispose();
                MaskImage.UnlockRectangle(0);
            }

            DXManager.TextureList.Add(this);
            TextureValid = true;
            Image.Disposing += (o, e) =>
            {
                TextureValid = false;
                Image = null;
                MaskImage = null;
                Data = null;
                DXManager.TextureList.Remove(this);
            };


            CleanTime = CMain.Time + Settings.CleanDelay;
        }
        public unsafe bool VisiblePixel(Point p, bool acurrate)
        {
            if (p.X < 0 || p.Y < 0 || p.X >= Width || p.Y >= Height)
                return false;

            int w = Width + (4 - Width % 4) % 4;

            bool result = false;
            if (Data != null)
            {
                int x = (p.X - p.X%4)/4;
                int y = (p.Y - p.Y%4)/4;
                int index = (y*(w/4) + x)*8;

                int col0 = (Data[index + 1] << 8 | Data[index]), col1 = (Data[index + 3] << 8 | Data[index + 2]);

                if (col0 == 0 && col1 == 0) return false;
                    
                if (!acurrate || col1 < col0) return true;
                
                x = p.X % 4;
                y = p.Y % 4;
                x *= 2;

                result = ((Data[index + 4 + y] & 1 << x) >> x) != 1 || ((Data[index + 4 + y] & 1 << x + 1) >> x + 1) != 1;
            }
            return result;
        }

        public Size GetTrueSize()
        {
            if (TrueSize != Size.Empty) return TrueSize;
            
            int l = 0, t = 0, r = Width, b = Height;

            bool visible = false;
            for (int x = 0; x < r; x++)
            {
                for (int y = 0; y < b; y++)
                {
                    if (!VisiblePixel(new Point(x, y), true)) continue;

                    visible = true;
                    break;
                }

                if (!visible) continue;

                l = x;
                break;
            }

            visible = false;
            for (int y = 0; y < b; y++)
            {
                for (int x = l; x < r; x++)
                {
                    if (!VisiblePixel(new Point(x, y), true)) continue;

                    visible = true;
                    break;

                }
                if (!visible) continue;

                t = y;
                break;
            }

            visible = false;
            for (int x = r - 1; x >= l; x--)
            {
                for (int y = 0; y < b; y++)
                {
                    if (!VisiblePixel(new Point(x, y), true)) continue;

                    visible = true;
                    break;
                }

                if (!visible) continue;

                r = x + 1;
                break;
            }

            visible = false;
            for (int y = b - 1; y >= t; y--)
            {
                for (int x = l; x < r; x++)
                {
                    if (!VisiblePixel(new Point(x, y), true)) continue;

                    visible = true;
                    break;

                }
                if (!visible) continue;

                b = y + 1;
                break;
            }

            TrueSize = Rectangle.FromLTRB(l, t, r, b).Size;

            return TrueSize;
        }
    }


}