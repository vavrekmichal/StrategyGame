using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miyagi.UI.Controls.Layout;
using Miyagi.Common.Data;
using Miyagi.Common.Resources;
using Miyagi.Common;
using Miyagi.UI;
using Miyagi.UI.Controls;
using Miyagi;

namespace Strategy.GameGUI {
	class MyGUI {
		protected int screenWidth;
		protected int screenHeight;

		Dictionary<string, Skin> skinDict;
		Dictionary<string, Font> fonts;
		MiyagiSystem system;
		GUI gui;
		FlowLayoutPanel mainMenu;

		public MyGUI(int w,int h, MOIS.Mouse m, MOIS.Keyboard k) {
			screenHeight = h;
			screenWidth = w;

			system = new MiyagiSystem("Mogre");
			gui = new GUI();
			system.GUIManager.GUIs.Add(gui);
			system.PluginManager.LoadPlugin(@"Miyagi.Plugin.Input.Mois.dll", m, k);

			fonts = new Dictionary<string, Font>();
			foreach (Font font in TrueTypeFont.CreateFromXml("../../Media/TrueTypeFonts.xml", system))
				fonts.Add(font.Name, font);
			Font.Default = fonts["BlueHighway"];

			IList<Skin> skins = Skin.CreateFromXml("../../Media/testSkin_map.skin");    //also an xml file, just a different extension
			skinDict = new Dictionary<string, Skin>();

			foreach (Skin skin in skins) {
				skinDict.Add(skin.Name, skin);
			}

			createMainMenu();
			createTopMenu();
			createCameraBounds();
		}

        /// <summary>
        /// Function creates invisible bounds for moving with world
        /// </summary>
		private void createCameraBounds() {
			var topSection = new Button() {
				Size = new Size(screenWidth*13/15, screenHeight/10),
				Location = new Point(screenWidth / 15, 0),
				Name="topSection"
			};
			topSection.MouseHover += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseOver);
			topSection.MouseLeave += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseLeave);
			gui.Controls.Add(topSection);

			var backSection = new Button() {
				Size = new Size(screenWidth*13/15, screenHeight / 10),
				Location = new Point(screenWidth / 15, screenHeight * 7 / 10),
				Name = "backSection"
			};
			backSection.MouseHover += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseOver);
			backSection.MouseLeave += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseLeave);
			gui.Controls.Add(backSection);

			var leftSection = new Button() {
				Size = new Size(screenWidth / 15, screenHeight - (screenHeight * 4 / 10)),
				Location = new Point(0, screenHeight / 10),
				Name = "leftSection"
			};
			leftSection.MouseHover += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseOver);
			leftSection.MouseLeave += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseLeave);
			gui.Controls.Add(leftSection);

			var rightSection = new Button() {
				Size = new Size(screenWidth /15, screenHeight - (screenHeight * 4 / 10)),
				Location = new Point(screenWidth * 14 / 15, screenHeight / 10),
				Name = "rightSection"
			};
			rightSection.MouseHover += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseOver);
			rightSection.MouseLeave += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseLeave);
			gui.Controls.Add(rightSection);

			var rightUpSection = new Button() {
				Size = new Size(screenWidth / 15, screenHeight /10),
				Location = new Point(screenWidth * 14 / 15, 0),
				Name = "rightUpSection"
			};
			rightUpSection.MouseHover += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseOver);
			rightUpSection.MouseLeave += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseLeave);
			gui.Controls.Add(rightUpSection);

			var rightDownSection = new Button() {
				Size = new Size(screenWidth / 15, screenHeight / 10),
				Location = new Point(screenWidth * 14 / 15, screenHeight * 7 / 10),
				Name = "rightDownSection"
			};
			rightDownSection.MouseHover += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseOver);
			rightDownSection.MouseLeave += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseLeave);
			gui.Controls.Add(rightDownSection);

			var leftDownSection = new Button() {
				Size = new Size(screenWidth / 15, screenHeight / 10),
				Location = new Point(0, screenHeight * 7 / 10),
				Name = "leftDownSection"
			};
			leftDownSection.MouseHover += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseOver);
			leftDownSection.MouseLeave += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseLeave);
			gui.Controls.Add(leftDownSection);

			var leftUpSection = new Button() {
				Size = new Size(screenWidth / 15, screenHeight / 10),
				Location = new Point(0, 0),
				Name = "leftUpSection"
			};
			leftUpSection.MouseHover += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseOver);
			leftUpSection.MouseLeave += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseLeave);
			gui.Controls.Add(leftUpSection);
		}

        /// <summary>
        /// Creates top bar
        /// </summary>
		private void createTopMenu(){
			Button upperMenu = new Button() {
				Size = new Size(screenWidth*18/20, screenHeight / 20),
				Location = new Point(screenWidth  / 20, 0),
				Skin = skinDict["Panel"],
			};
			gui.Controls.Add(upperMenu);
		
		}

        /// <summary>
        /// Creates main bar
        /// </summary>
		private void createMainMenu() {
			mainMenu = new FlowLayoutPanel() {
				Size = new Size(screenWidth, screenHeight / 5),
				Skin = skinDict["PanelR"],
				Location = new Point(0, screenHeight * 8 / 10)
			};
			gui.Controls.Add(mainMenu);
			for (int i = 0; i < 3; i++) {
				mainMenu.Controls.Add(new Button() {
					Size = new Size((int)mainMenu.Width / 3, (int)mainMenu.Height),
					//Location = new Point(50, (int)mainMenu.Width * i / 4),
					Skin = skinDict["PanelR"],
					//HitTestVisible = false
				});
			}

			
			mainMenu.Controls[0].Controls.Add(new Button() {
				Size = new Size(140,40),
				Skin = skinDict["Button"],
				Text = "  EXIT BUTTON"
			});
			mainMenu.Controls[0].Controls[0].MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(quitOnClick);

			mainMenu.Controls[1].Controls.Add(new Label() {
				Size = new Size (140,50),
				Text = " bla"
				
			});

			mainMenu.Controls[2].Controls.Add(new Label() {
				Size = new Size(140, 50),
				Text = " bla"

			});

			
			//Button button = new Button();
			//button.Size = new Size(200, 100);
			//button.Location = new Point(50, 500);
			//button.Skin = skinDict["Button"];
			//gui.Controls.Add(button);

			//// Label
			//Label label = new Label();
			//label.Size = new Size(100, 50);
			//label.Location = new Point(500, 500);
			//label.Text = "Hello Miyagi";
			//gui.Controls.Add(label);

			// Cursor
			Skin cursorSkin = Skin.CreateFromXml("../../Media/cursorSkin.xml")[0];
			Cursor cursor = new Cursor(cursorSkin, new Size(30, 30), new Point(0, 0), true);
			
			//cursor.
			system.GUIManager.Cursor = cursor;

			// Progressbar
			//progress = new ProgressBar();
			//progress.Size = new Size(300, 50);
			//progress.Skin = skinDict["ProgressBarH"];
			//progress.Location = new Point(50, 50);
			//gui.Controls.Add(progress);

			//// Table
			//TableLayoutPanel table = new TableLayoutPanel();
			//table.Location = new Point(50, 300);
			//table.Size = new Size(500, 500);

			//table.RowCount = 5;
			//table.ColumnCount = 5;

			//// Make sure you create the TableLayoutStyle objects, and add them to the table's column styles and row styles
			//// (dont need to do this for flow layout)
			//var colStyles = new TableLayoutStyle[table.RowCount];
			//var rowStyles = new TableLayoutStyle[table.ColumnCount];

			//for (int i = 0; i < colStyles.Length; i++)
			//	colStyles[i] = new TableLayoutStyle(SizeType.Absolute, 60); //width

			//for (int i = 0; i < rowStyles.Length; i++)
			//	rowStyles[i] = new TableLayoutStyle(SizeType.Absolute, 50); //height

			//table.ColumnStyles.AddRange(colStyles);
			//table.RowStyles.AddRange(rowStyles);

			//for (int i = 0; i < table.RowCount + table.ColumnCount; i++) {
			//	Button b = new Button();
			//	b.Size = new Size(50, 50);
			//	b.Skin = skinDict["Button"];
			//	table.Controls.Add(b);
			//}

			//gui.Controls.Add(table);

			//// Flow layout panel
			//FlowLayoutPanel flowPanel = new FlowLayoutPanel();
			//flowPanel.Size = new Size(300, 500);
			//flowPanel.Location = new Point(600, 50);
			//flowPanel.Skin = skinDict["Panel"];

			//for (int i = 0; i < 5; i++) {
			//	Button b = new Button();
			//	b.Size = new Size(50, 50);
			//	b.Skin = skinDict["Button"];
			//	flowPanel.Controls.Add(b);
			//}

			//Button b2 = new Button();
			//b2.Size = new Size(80, 80);         // Put in the middle to see the effect it has on the flow panel
			//b2.Skin = skinDict["Button"];
			//b2.Text = "EXIT BUTTON";
			//b2.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(quitOnClick);
			//flowPanel.Controls.Add(b2);

			//for (int i = 0; i < 5; i++) {
			//	Button b = new Button();
			//	b.Size = new Size(50, 50);
			//	b.Skin = skinDict["Button"];
			//	flowPanel.Controls.Add(b);
			//}

			//gui.Controls.Add(flowPanel);
		}

		//Button Actions
		private void quitOnClick(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			system.Dispose();
			throw new ShutdownException();
		}

		private void mouseOver(object sender, Miyagi.Common.Events.MouseEventArgs e) {
			MogreControl.MouseControl.move(((Button)sender).Name);
			
		}

		private void mouseLeave(object sender, Miyagi.Common.Events.MouseEventArgs e) {
			MogreControl.MouseControl.moveStop(((Button)sender).Name);
		}


        /// <summary>
        /// Dispose GUI system
        /// </summary>
		public void dispose() {
			system.Dispose();
		}

        /// <summary>
        /// Function updete GUI
        /// </summary>
		public void update() {
			system.Update();
		}


		///
		///
		///pokusy
		///
		public void showTargeted(string s) {
			((Label)mainMenu.Controls[1].Controls[0]).Text = s;
		}
	}
}
