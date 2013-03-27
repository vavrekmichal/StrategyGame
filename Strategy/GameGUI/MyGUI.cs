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
using Strategy.GameMaterial;
using Miyagi.UI.Controls.Styles;
using Strategy.GroupControl;

namespace Strategy.GameGUI {
	class MyGUI {
		//TODO: udelat dictionary s string a label pro nastavovani stavu surovin.
		protected int screenWidth;
		protected int screenHeight;

		protected FlowLayoutPanel upperMenu;
		protected FlowLayoutPanel mainMenu;
		protected Label nameOfSolarSystem;

		private Panel buttonsPanel;
		private Panel statPanel;
		private Panel actionPanel;

		//statPanel members
		private Label statPanelName;
		private Label statPanelMesh;
		private Label statPanelTeam;

		Dictionary<string, Skin> skinDict;
		Dictionary<string, Font> fonts;
		MiyagiSystem system;
		GUI gui;
		GroupManager groupMgr;

		protected Dictionary<string, MaterialGUIPair> materialList;

		public MyGUI(int w, int h, MOIS.Mouse m, MOIS.Keyboard k, Dictionary<string, IMaterial> materials, GroupManager groupMgr) {
			this.groupMgr = groupMgr;
			materialList = new Dictionary<string, MaterialGUIPair>();
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
			createTopMenu(materials);

			createCameraBounds();
		}

		/// <summary>
		/// Function creates invisible bounds for moving with world
		/// </summary>
		private void createCameraBounds() {
			var topSection = new Button() {
				Size = new Size(screenWidth * 13 / 15, screenHeight / 10),
				Location = new Point(screenWidth / 15, screenHeight * 2 / 30),
				Name = "topSection"
			};
			topSection.MouseHover += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseOver);
			topSection.MouseLeave += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseLeave);
			gui.Controls.Add(topSection);

			var backSection = new Button() {
				Size = new Size(screenWidth * 13 / 15, screenHeight / 10),
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
				Size = new Size(screenWidth / 15, screenHeight - (screenHeight * 4 / 10)),
				Location = new Point(screenWidth * 14 / 15, screenHeight / 10),
				Name = "rightSection"
			};
			rightSection.MouseHover += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseOver);
			rightSection.MouseLeave += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseLeave);
			gui.Controls.Add(rightSection);

			var rightUpSection = new Button() {
				Size = new Size(screenWidth / 15, screenHeight / 10),
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
		/// <param name="materials">Players materials</param>
		private void createTopMenu(Dictionary<string, IMaterial> materials) {
			upperMenu = new FlowLayoutPanel() {
				Size = new Size(screenWidth * 18 / 20, screenHeight * 2 / 30),
				Location = new Point(screenWidth / 20, 0),
				Skin = skinDict["Panel"],
				Opacity = 0.5f
			};
			gui.Controls.Add(upperMenu);

			Label label1 = new Label() {
				Name = "nameOfSolarSystem",
				Size = new Size(upperMenu.Width / 6, upperMenu.Height * 4 / 5),
				Text = "Current solar system: ",
				TextStyle = {
					Alignment = Miyagi.Common.Alignment.MiddleLeft,
					ForegroundColour = Colours.White
				},
				Padding = new Thickness(10, 0, 0, 0)
			};
			upperMenu.Controls.Add(label1);

			nameOfSolarSystem = new Label() {
				Name = "nameOfSolarSystem",
				Size = new Size(upperMenu.Width / 5, upperMenu.Height * 4 / 5),
				Text = "Booted system ",
				TextStyle = {
					Alignment = Miyagi.Common.Alignment.MiddleLeft,
					ForegroundColour = Colours.Black
				},
				Padding = new Thickness(1)
			};
			upperMenu.Controls.Add(nameOfSolarSystem);

			Label materialIntro = new Label() {
				Name = "MaterialState",
				Size = new Size(upperMenu.Width / 6, upperMenu.Height * 4 / 5),
				Text = "Material State: ",
				TextStyle = {
					Alignment = Miyagi.Common.Alignment.MiddleLeft,
					ForegroundColour = Colours.White
				},
				Padding = new Thickness(1)
			};
			upperMenu.Controls.Add(materialIntro);

			//Material load
			var materialBox = new Panel() {
				Size = new Size(upperMenu.Width / 3, upperMenu.Height),
				ResizeMode = ResizeModes.None,
				Skin = skinDict["PanelSkin"],
				Opacity = 0.5f,
				TabStop = false,
				TabIndex = 0,
				Throwable = true,
				ResizeThreshold = new Thickness(8),
				BorderStyle = new BorderStyle { Thickness = new Thickness(2) },
				HScrollBarStyle = new ScrollBarStyle() {
					Extent = 16,
					ThumbStyle = {
						BorderStyle = {
							Thickness = new Thickness(2, 2, 2, 2)
						}
					}
				},
				VScrollBarStyle = new ScrollBarStyle {
					Extent = 12,
					ShowButtons = true,
					ThumbStyle = {
						BorderStyle = {
							Thickness = new Thickness(2, 2, 2, 2)
						}
					}
				}

			};

			int row = 0;
			foreach (KeyValuePair<string, IMaterial> k in materials) {
				materialList.Add(k.Key, new MaterialGUIPair(k.Key, k.Value.state, materialBox.Width, row));
				row++;
			}

			row = 0;
			foreach (KeyValuePair<string, MaterialGUIPair> valuePair in materialList) {
				materialBox.Controls.Add(valuePair.Value.name);
				materialBox.Controls.Add(valuePair.Value.value);
				row++;
			}

			upperMenu.Controls.Add(materialBox);
		}

		/// <summary>
		/// Change printed solar system name
		/// </summary>
		/// <param name="name">new printed name</param>
		public void setSolarSystemName(string name) {
			nameOfSolarSystem.Text = name;
		}

		public void showTargeted(GroupControl.GroupMovables group) {
			//statPanelName.Text = 
			statPanelName.Text = group[0].Name;
			statPanelTeam.Text = group[0].Team.Name;
			statPanelMesh.Text = group.Count.ToString();

		}

		public void showTargeted(GroupControl.GroupStatics group) {
			//Just one object
			if (group.Count==0) {
				statPanelName.Text = "Nothing selected";
				statPanelMesh.Text = "Nothing selected";
				statPanelTeam.Text = "Nothing selected";
				return;
			}
			statPanelTeam.Text = group[0].Team.Name;
			statPanelName.Text = group[0].Name;
			statPanelMesh.Text = group[0].Mesh;
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

			mainMenu.Controls.Add(createButtonPanel());
			mainMenu.Controls.Add(createStatPanel());
			mainMenu.Controls.Add(createActionPanel());

			// Cursor
			Skin cursorSkin = Skin.CreateFromXml("../../Media/cursorSkin.xml")[0];
			Cursor cursor = new Cursor(cursorSkin, new Size(30, 30), new Point(0, 0), true);

			//cursor.
			system.GUIManager.Cursor = cursor;
		}

		//Button Actions
		#region Button Actions
		private void selectSolarSystem(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			if (sender.GetType() == typeof(SelectionLabel)) {
				groupMgr.changeSolarSystem(((SelectionLabel)sender).NumberOfItem);
				((SelectionLabel)sender).PanelToClose.Dispose();
			}
		}

		private void quitOnClick(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			system.Dispose();
			throw new Strategy.Exceptions.ShutdownException();
		}

		private void pause(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			Game.pause(!Game.gameStatus());
		}

		private void mouseOver(object sender, Miyagi.Common.Events.MouseEventArgs e) {
			MogreControl.MouseControl.move(((Button)sender).Name);

		}

		private void mouseLeave(object sender, Miyagi.Common.Events.MouseEventArgs e) {
			MogreControl.MouseControl.moveStop(((Button)sender).Name);
		}

		private void disposePanel(object sender, Miyagi.Common.Events.MouseEventArgs e) {

			if (sender.GetType() == typeof(CloseButton)) {
				((CloseButton)sender).PanelToClose.Dispose();
			}
		}

		private void showSolarSystems(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			createSolarSystemPanel();
		}
		#endregion


		/// <summary>
		/// Main panel with three subPanels
		/// </summary>
		/// <returns>Main panel (Miyagi Panel)</returns>
		private Panel createMainmenuSubpanel() {
			var p = new Panel() {
				ResizeMode = ResizeModes.None,
				Padding = new Thickness(5),
				Size = new Size((int)mainMenu.Width / 3, (int)mainMenu.Height),
				Skin = skinDict["PanelR"]
			};
			return p;
		}



		/// <summary>
		/// Creates top bar with info about current SolarSystem and players materials
		/// </summary>
		private void createSolarSystemPanel() {
			var panel = new Panel() {
				Width = screenWidth / 2,
				Height = screenHeight / 2,
				Location = new Point(screenWidth / 4, screenHeight / 4),
				Skin = skinDict["PanelR"],
				ResizeMode = ResizeModes.None
			};
			var label = new Label() {
				Size = new Size(panel.Width / 2, panel.Height / 10),
				Text = " Select solar system:",
				Location = new Point(panel.Width / 4, 5),
				TextStyle = {
					Alignment = Miyagi.Common.Alignment.TopCenter
				}
			};
			panel.Controls.Add(label);

			List<string> solarSystList = groupMgr.getSolarSystemNames();
			int marginTop = panel.Height / 10 + 10;
			for (int i = 0; i < solarSystList.Count; i++) {
				SelectionLabel selLabel = createSelectionLabel(
					panel.Width / 2, panel.Height / 10, solarSystList[i], 
					new Point(panel.Width / 4, marginTop * (i+1)), i, panel
					);
			
				panel.Controls.Add(selLabel);
				selLabel.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(selectSolarSystem); ;
			}


			var b = createCloseButton(panel.Width / 2, panel.Height / 10, "	Cancel", new Point(panel.Width / 2, panel.Height * 9 / 10), panel);
			b.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(disposePanel);
			panel.Controls.Add(b);
			gui.Controls.Add(panel);
		}

		/// <summary>
		/// create SelectionLabel - on click select positon and can close setted panel
		/// </summary>
		/// <param name="width">width</param>
		/// <param name="height">height</param>
		/// <param name="text">text in Label</param>
		/// <param name="location">relative position in Panel</param>
		/// <param name="order">number of choice</param>
		/// <param name="panelToClose">Closing Panel</param>
		/// <returns></returns>
		private SelectionLabel createSelectionLabel(int width, int height, string text, Point location, int order, Panel panelToClose) {
			var selectLabel = new SelectionLabel(order, panelToClose) {
				Size = new Size(width, height),
				Text = text,
				Location = location
			};
			return selectLabel;
		}


		private Button createButton(int width, int height, string text, Point location) {
			Button b = new Button() {
				Size = new Size(width, height),
				Location = location,
				Skin = skinDict["Button"],
				Text = text
			};
			return b;
		}

		private CloseButton createCloseButton(int width, int height, string text, Point location, Panel panelToClose) {
			var b = new CloseButton(panelToClose) {
				Size = new Size(width, height),
				Location = location,
				Skin = skinDict["Button"],
				Text = text
			};
			return b;
		}

		/// <summary>
		/// Create first panel in main panel. (Control buttons)
		/// </summary>
		/// <returns>Miyagi Panel</returns>
		private Panel createButtonPanel() {
			buttonsPanel = createMainmenuSubpanel();
			int buttonMarginLeft = buttonsPanel.Width / 4;
			int buttonMarginTop = buttonsPanel.Height * 6 / 25;
			int row = buttonsPanel.Height / 5 + 5;
			Button b = createButton(buttonsPanel.Width / 2, buttonsPanel.Height / 5, "  Pause BUTTON", new Point(buttonMarginLeft, 0));
			buttonsPanel.Controls.Add(b);
			b.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(pause);

			b = createButton(buttonsPanel.Width / 2, buttonsPanel.Height / 5, "  Exit BUTTON", new Point(buttonMarginLeft, buttonMarginTop));
			buttonsPanel.Controls.Add(b);
			b.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(quitOnClick);

			b = createButton(buttonsPanel.Width / 2, buttonsPanel.Height / 5, "  Window", new Point(buttonMarginLeft, buttonMarginTop * 2));
			buttonsPanel.Controls.Add(b);
			b.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(showSolarSystems);
			return buttonsPanel;
		}

		/// <summary>
		/// Create second panel in main panel
		/// </summary>
		/// <returns>Miyagi Panel</returns>
		private Panel createStatPanel() {
			statPanel = createMainmenuSubpanel();
			int x1 = statPanel.Width / 3;
			int y = 50;
			//Name
			statPanel.Controls.Add(new Label() {
				Size = new Size(x1, y),
				Text = " Name: ",
				Padding = new Thickness(5)

			});
			int x2 = statPanel.Width * 2 / 3;
			statPanelName = new Label() {
				Size = new Size(x2, y),
				Location = new Point(statPanel.Width * 2 / 8, 0),
				Text = " Jeste neni",
				Padding = new Thickness(5)
			};
			statPanel.Controls.Add(statPanelName);

			//Mesh
			statPanel.Controls.Add(new Label() {
				Size = new Size(x1, y),
				Location = new Point(0, y),
				Text = " Mesh: ",
				Padding = new Thickness(5)

			});

			statPanelMesh = new Label() {
				Size = new Size(x2, y),
				Location = new Point(statPanel.Width * 2 / 8, y),
				Text = " Jeste neni",
				Padding = new Thickness(5)
			};
			statPanel.Controls.Add(statPanelMesh);

			//Team
			statPanel.Controls.Add(new Label() {
				Size = new Size(x1, y),
				Location = new Point(0, 2*y),
				Text = " Team: ",
				Padding = new Thickness(5)

			});

			statPanelTeam = new Label() {
				Size = new Size(x2, y),
				Location = new Point(statPanel.Width * 2 / 8, 2 * y),
				Text = " Jeste neni",
				Padding = new Thickness(5)
			};
			statPanel.Controls.Add(statPanelTeam);
			return statPanel;
		}

		/// <summary>
		/// Create third panel in main panel
		/// </summary>
		/// <returns>Miyagi Panel</returns>
		private Panel createActionPanel() {
			actionPanel = createMainmenuSubpanel();
			actionPanel.Controls.Add(new Label() {
				Size = new Size(140, 50),
				Text = " Developing",
				Padding = new Thickness(5)

			});
			return actionPanel;
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

		public void setMaterialState(string material, int inc) {
			if (materialList.ContainsKey(material)) {
				materialList[material].value.Text = (inc).ToString();
			} else {
				throw new Strategy.Exceptions.MissingMaterialException("This Material is not in your list. You can not set value to nonexist material.");
			}
		}
	}
}
