using System;
using System.Collections.Generic;
using Miyagi.Common.Data;
using Miyagi.Common.Resources;
using Miyagi.Common;
using Miyagi.UI;
using Miyagi.UI.Controls;
using Miyagi.UI.Controls.Styles;
using Strategy.GameMaterial;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.GameObjectControl.RuntimeProperty;
using System.Reflection;
using Strategy.GameObjectControl.Game_Objects.GameActions;

namespace Strategy.GameGUI {
	public class MyGUI : IGameGUI {
		//TODO: udelat dictionary s string a label pro nastavovani stavu surovin.
		protected int screenWidth;
		protected int screenHeight;

		protected FlowLayoutPanel upperMenu;
		protected FlowLayoutPanel mainMenu;
		protected Label nameOfSolarSystem;

		private Panel buttonsPanel;
		private Panel statPanel;
		private Panel actionPanel;

		private Label statPanelName;
		private Panel propertyPanel;

		private Panel gameActionPanel;

		private Panel console;
		private int consoleLinesNumber;

		Dictionary<string, Skin> skinDict;
		Dictionary<string, Font> fonts;
		MiyagiSystem system;
		GUI gui;

		private const string nothingSelected = "	Nothing selected";
		private const int textHeight = 18;

		protected Dictionary<string, MaterialGUIPair> materialList;

		/// <summary>
		/// Constructor initializes GUI systrem for Mogre and Load fonts, skins and create control panel.
		/// Also the constuctor creates mouse bounds (app using Miagi cursor for control)
		/// </summary>
		/// <param Name="width">Window width</param>
		/// <param Name="height">Window height</param>
		/// <param Name="mouse">Mogre mouse input</param>
		/// <param Name="keyboard">Mogre keyboard input</param>
		/// <param Name="materials">Names and values of user materials</param>
		/// <param Name="groupMgr">GroupManager instance</param>
		public MyGUI(int width, int height, MOIS.Mouse mouse, MOIS.Keyboard keyboard) {

			materialList = new Dictionary<string, MaterialGUIPair>();
			screenHeight = height;
			screenWidth = width;

			// Set Miyagi to Mogre
			system = new MiyagiSystem("Mogre");
			gui = new GUI();
			system.GUIManager.GUIs.Add(gui);
			system.PluginManager.LoadPlugin(@"Miyagi.Plugin.Input.Mois.dll", mouse, keyboard);


			// Font loading from xml file
			fonts = new Dictionary<string, Font>();
			foreach (Font font in TrueTypeFont.CreateFromXml("../../Media/TrueTypeFonts.xml", system))
				fonts.Add(font.Name, font);
			Font.Default = fonts["BlueHighway"];


			// Also an xml file, just a different extension
			IList<Skin> skins = Skin.CreateFromXml("../../Media/testSkin_map.skin");
			skinDict = new Dictionary<string, Skin>();

			foreach (Skin skin in skins) {
				skinDict.Add(skin.Name, skin);
			}

			// Load cursor and skin from xml file
			Skin cursorSkin = Skin.CreateFromXml("../../Media/cursorSkin.xml")[0];
			Cursor cursor = new Cursor(cursorSkin, new Size(30, 30), new Point(0, 0), true);

			system.GUIManager.Cursor = cursor;   // Set cursor

			CreateMainMenu();
			CreateTopMenu();

			CreateCameraBounds();				// Create bounds around screen to move the camera
		}

		public void Inicialization() {
			LoadMaterials(Game.TeamManager.GetPlayersMaterials());
		}

		/// <summary>
		/// Function creates invisible bounds for moving with camera
		/// </summary>
		private void CreateCameraBounds() {

			// Top bound
			var topSection = new Button() {
				Size = new Size(screenWidth * 13 / 15, screenHeight / 10),
				Location = new Point(screenWidth / 15, screenHeight * 2 / 30),
				Name = "topSection"
			};
			topSection.MouseHover += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(MouseOver);
			topSection.MouseLeave += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(MouseLeave);
			gui.Controls.Add(topSection);

			// Botton bound
			var backSection = new Button() {
				Size = new Size(screenWidth * 13 / 15, screenHeight / 10),
				Location = new Point(screenWidth / 15, screenHeight * 7 / 10),
				Name = "backSection"
			};
			backSection.MouseHover += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(MouseOver);
			backSection.MouseLeave += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(MouseLeave);
			gui.Controls.Add(backSection);

			// Left bound
			var leftSection = new Button() {
				Size = new Size(screenWidth / 15, screenHeight - (screenHeight * 4 / 10)),
				Location = new Point(0, screenHeight / 10),
				Name = "leftSection"
			};
			leftSection.MouseHover += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(MouseOver);
			leftSection.MouseLeave += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(MouseLeave);
			gui.Controls.Add(leftSection);

			// Right bound
			var rightSection = new Button() {
				Size = new Size(screenWidth / 15, screenHeight - (screenHeight * 4 / 10)),
				Location = new Point(screenWidth * 14 / 15, screenHeight / 10),
				Name = "rightSection"
			};
			rightSection.MouseHover += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(MouseOver);
			rightSection.MouseLeave += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(MouseLeave);
			gui.Controls.Add(rightSection);

			// Rigth-up corner bound
			var rightUpSection = new Button() {
				Size = new Size(screenWidth / 15, screenHeight / 10),
				Location = new Point(screenWidth * 14 / 15, 0),
				Name = "rightUpSection"
			};
			rightUpSection.MouseHover += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(MouseOver);
			rightUpSection.MouseLeave += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(MouseLeave);
			gui.Controls.Add(rightUpSection);

			// Right-down corner bound
			var rightDownSection = new Button() {
				Size = new Size(screenWidth / 15, screenHeight / 10),
				Location = new Point(screenWidth * 14 / 15, screenHeight * 7 / 10),
				Name = "rightDownSection"
			};
			rightDownSection.MouseHover += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(MouseOver);
			rightDownSection.MouseLeave += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(MouseLeave);
			gui.Controls.Add(rightDownSection);

			// Left-down corner bound
			var leftDownSection = new Button() {
				Size = new Size(screenWidth / 15, screenHeight / 10),
				Location = new Point(0, screenHeight * 7 / 10),
				Name = "leftDownSection"
			};
			leftDownSection.MouseHover += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(MouseOver);
			leftDownSection.MouseLeave += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(MouseLeave);
			gui.Controls.Add(leftDownSection);

			// Left-up corner bound
			var leftUpSection = new Button() {
				Size = new Size(screenWidth / 15, screenHeight / 10),
				Location = new Point(0, 0),
				Name = "leftUpSection"
			};
			leftUpSection.MouseHover += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(MouseOver);
			leftUpSection.MouseLeave += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(MouseLeave);
			gui.Controls.Add(leftUpSection);
		}



		/// <summary>
		/// Change printed solar system Name
		/// </summary>
		/// <param Name="Name">New printed SolarSystem's Name</param>
		public void SetSolarSystemName(string name) {
			nameOfSolarSystem.Text = name;
		}

		/// <summary>
		/// Function shows every given Property from in propertisDict in propertyPanel
		/// </summary>
		/// <param Name="propertiesDict">Dictionary with properties (key - Name, value - Property)</param>
		private void ShowGroupProperties(Dictionary<string, object> propertiesDict) {
			var propDict = propertiesDict;
			int marginLeft = propertyPanel.Width / 2;
			int marginTop = 26;
			int i = 0;

			// Add each property int propertyPanel Name and value in one row
			foreach (var property in propDict) {
				propertyPanel.Controls.Add(CreateLabel(marginTop * i, marginLeft, 26, property.Key));

				propertyPanel.Controls.Add((Label)CreatePropertyLabelAsObject(marginLeft, marginTop * i, property.Value));
				++i;
			}
		}


		/// <summary>
		/// Function shows Group's property and count or Name of object (if is just one in the group)
		/// </summary>
		/// <param Name="group">Showing group with IMovableGameObjects</param>
		public void ShowTargeted(GroupMovables group) {
			ClearGroupPanels();

			if (group.Count == 1) { // Just one object
				statPanelName.Text = group[0].Name;
			} else {
				statPanelName.Text = "Count: " + group.Count;
			}
			ShowGroupProperties(group.GetPropertyToDisplay());

			if (group.OwnerTeam.Name == Game.playerName) {
				ShowIGameActionIcons(group.GetGroupIGameActions());
			}
		}


		/// <summary>
		/// Function shows Group's property and count or Name of object (if is just one in the group)
		/// Also can show nothingSelect constant if group is empty
		/// </summary>
		/// <param Name="group"></param>
		public void ShowTargeted(GroupStatics group) {

			ClearGroupPanels();
			if (group == null || group.Count == 0) {
				statPanelName.Text = nothingSelected;
				propertyPanel.Controls.Add(new Label() {
					Text = nothingSelected,
					Location = new Point(0, 0),
					Size = new Size(propertyPanel.Width, propertyPanel.Height)
				});
				return;
			}

			ShowGroupProperties(group.GetPropertyToDisplay());
			if (group.OwnerTeam.Name == Game.playerName) {
				ShowIGameActionIcons(group.GetGroupIGameActions());
			}
			if (group.Count == 1) { // Just one object
				var isgo = group[0];
				statPanelName.Text = isgo.Name;


			} else {// Player's planets (only way when is selected more than 1 IStaticGameObjects -> display actions
				statPanelName.Text = "Count: " + group.Count;
			}

		}

		/// <summary>
		/// Creeates generic PropertyLabel with runtime setted type
		/// </summary>
		/// <param Name="marginLeft">Left indent</param>
		/// <param Name="marginTop">Top indent</param>
		/// <param Name="property">Property </param>
		/// <returns>PropertyLabel as object</returns>
		private object CreatePropertyLabelAsObject(int marginLeft, int marginTop, object property) {
			MethodInfo method = typeof(MyGUI).GetMethod("CreatePropertyLabelAsLabel", BindingFlags.NonPublic | BindingFlags.Instance); //CreatePropertyLabelAsLabel is private function
			var type = property.GetType().GetGenericArguments()[0];
			MethodInfo generic = method.MakeGenericMethod(type);
			List<object> args = new List<object>();
			args.Add(marginLeft);
			args.Add(marginTop);
			args.Add(marginLeft);
			args.Add(26);
			args.Add(property);
			var o = generic.Invoke(this, args.ToArray());
			return o;
		}

		#region top panel
		/// <summary>
		/// Creates top bar
		/// </summary>
		/// <param Name="materials">Players materials</param>
		private void CreateTopMenu() {
			upperMenu = new FlowLayoutPanel() {				//create top panel (empty blue box)
				Size = new Size(screenWidth * textHeight / 20, screenHeight * 2 / 30),
				Location = new Point(screenWidth / 20, 0),
				Skin = skinDict["Panel"],
				Opacity = 0.5f
			};
			gui.Controls.Add(upperMenu);

			Label label1 = new Label() {					//Left Label with text "Current solar system:"
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

			nameOfSolarSystem = new Label() {				//Label to show actual solar system Name
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

			Label materialIntro = new Label() {				//Right Label with text "Material State:"
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
		}

		public void LoadMaterials(Dictionary<string, IMaterial> materials) {
			//Material Load - create one line with Name of IMaterial and value
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
					ShowButtons = true,
					ThumbStyle = {
						BorderStyle = {
							Thickness = new Thickness(2, 2, 2, 2)
						}
					}
				},
				VScrollBarStyle = new ScrollBarStyle {
					Extent = 12,
					ThumbStyle = {
						BorderStyle = {
							Thickness = new Thickness(2, 2, 2, 2)
						}
					}
				}

			};

			int row = 0;
			foreach (KeyValuePair<string, IMaterial> k in materials) {			//creates pair Name - value
				materialList.Add(k.Key, new MaterialGUIPair(k.Key, k.Value.State, materialBox.Width, row));
				row++;
			}

			row = 0;
			foreach (KeyValuePair<string, MaterialGUIPair> valuePair in materialList) {		//Set pairs into GUI
				materialBox.Controls.Add(valuePair.Value.name);
				materialBox.Controls.Add(valuePair.Value.value);
				row++;
			}

			upperMenu.Controls.Add(materialBox);
		}

		#endregion
		#region main panel

		/// <summary>
		/// Creates main bar
		/// </summary>
		private void CreateMainMenu() {
			mainMenu = new FlowLayoutPanel() {

				Size = new Size(screenWidth, screenHeight / 5),
				Skin = skinDict["PanelR"],
				Location = new Point(0, screenHeight * 8 / 10)
			};
			gui.Controls.Add(mainMenu);

			mainMenu.Controls.Add(CreateButtonPanel());
			mainMenu.Controls.Add(CreateStatPanel());
			mainMenu.Controls.Add(CreateActionPanel());

		}

		/// <summary>
		/// Create first panel in main panel. (Control buttons)
		/// </summary>
		/// <returns>Miyagi Panel</returns>
		private Panel CreateButtonPanel() {
			buttonsPanel = CreateMainmenuSubpanel();
			int buttonMarginLeft = buttonsPanel.Width / 4;
			int buttonMarginTop = buttonsPanel.Height * 6 / 25;
			int row = buttonsPanel.Height / 5 + 5;
			Button b = CreateButton(buttonsPanel.Width / 2, buttonsPanel.Height / 5, "  Pause BUTTON", new Point(buttonMarginLeft, 0));
			buttonsPanel.Controls.Add(b);
			b.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(Pause);
			b = CreateButton(buttonsPanel.Width / 2, buttonsPanel.Height / 5, "  Solar systems", new Point(buttonMarginLeft, buttonMarginTop));
			buttonsPanel.Controls.Add(b);
			b.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(ShowSolarSystems);

			b = CreateButton(buttonsPanel.Width / 2, buttonsPanel.Height / 5, "  Exit BUTTON", new Point(buttonMarginLeft, buttonMarginTop * 2));
			buttonsPanel.Controls.Add(b);
			b.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(QuitOnClick);


			return buttonsPanel;
		}

		/// <summary>
		/// Create second panel in main panel
		/// </summary>
		/// <returns>Miyagi Panel</returns>
		private Panel CreateStatPanel() {
			statPanel = CreateMainmenuSubpanel();
			int x1 = statPanel.Width / 3;
			int y = 40;

			statPanel.Controls.Add(new Label() {
				Size = new Size(x1, y),
				Text = " Selected: ",
				Padding = new Thickness(5)

			});
			int x2 = statPanel.Width * 2 / 3;
			statPanelName = new Label() {
				Size = new Size(x2, y),
				Location = new Point(statPanel.Width * 2 / 8, 0),
				Padding = new Thickness(5)
			};
			statPanel.Controls.Add(statPanelName);

			// Minus 30 is padding correction (2*5 panel padding + 2*10 scrollablePanel padding)
			propertyPanel = CreateInnerScrollablePanel(y, statPanel.Width - 30, statPanel.Height - (y + 10));
			statPanel.Controls.Add(propertyPanel);

			return statPanel;

		}

		/// <summary>
		/// Clear panel with descriptions (properties) and with actions
		/// </summary>
		private void ClearGroupPanels() {
			propertyPanel.Controls.Clear();
			gameActionPanel.Controls.Clear();
		}

		/// <summary>
		/// Create third panel in main panel
		/// </summary>
		/// <returns>Miyagi Panel</returns>
		private Panel CreateActionPanel() {
			actionPanel = CreateMainmenuSubpanel();
			//actionPanel.Controls.Add(new Label() {
			//	Size = new Size(140, 50),
			//	Text = " Developing",
			//	Padding = new Thickness(5)

			//});
			gameActionPanel = CreateInnerScrollablePanel(5, actionPanel.Width - 30, actionPanel.Height / 2 - 10);
			gameActionPanel.Padding = new Thickness(2, 2, 0, 0);

			actionPanel.Controls.Add(gameActionPanel);
			console = CreateInnerScrollablePanel(actionPanel.Height / 2 + 1, actionPanel.Width - 30, actionPanel.Height / 2 - 10);

			actionPanel.Controls.Add(console);

			return actionPanel;
		}


		#endregion

		#region Button Actions

		private void GameActionClicked(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			var icon = sender as GameActionIconBox;
			if (icon != null) {
				icon.MouseClicked();
			}
		}

		private void SelectSolarSystem(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			// Button Action for SelectionLabel calls GroupManager function ChangeSolarSystem and close Panel
			var selectionLabel = sender as SelectionLabel;
			if (selectionLabel != null) {
				Game.GroupManager.ChangeSolarSystem(selectionLabel.NumberOfItem);
				selectionLabel.ClosePanel();
			}
		}

		private void Travel(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			// Function calls CreateTraveler with selected number of SolarSystem and traveler(IMovableGameObject)
			var selectionLabel = sender as SelectionLabel;
			if (selectionLabel != null) {
				Game.GroupManager.CreateTraveler(selectionLabel.NumberOfItem, selectionLabel.StoredObject);
				selectionLabel.ClosePanel();
			}
		}

		private void QuitOnClick(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			// Function closes application (throw ShutdownException)
			system.Dispose();
			throw new Strategy.Exceptions.ShutdownException();
		}

		private void Pause(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			Game.Pause(!Game.GameStatus);
		}

		private void MouseOver(object sender, Miyagi.Common.Events.MouseEventArgs e) {
			// Controls camera move
			MogreControl.MouseControl.Move(((Button)sender).Name);

		}

		private void MouseLeave(object sender, Miyagi.Common.Events.MouseEventArgs e) {
			// Controls camera move
			MogreControl.MouseControl.MoveStop(((Button)sender).Name);
		}

		private void DisposePanel(object sender, Miyagi.Common.Events.MouseEventArgs e) {
			// On click button Dispose the panel 
			var closeButton = sender as CloseButton;
			if (closeButton != null) {
				closeButton.ClosePanel();
			}
		}

		private void ShowSolarSystems(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			// Shows Panel with SolarSystems and travelers
			CreateSolarSystemPanel();
		}
		#endregion


		/// <summary>
		/// Main panel with three subPanels
		/// </summary>
		/// <returns>Main panel (Miyagi Panel)</returns>
		private Panel CreateMainmenuSubpanel() {
			var p = new Panel() {
				ResizeMode = ResizeModes.None,
				Padding = new Thickness(5),
				Size = new Size((int)mainMenu.Width / 3, (int)mainMenu.Height),
				Skin = skinDict["PanelR"]
			};
			return p;
		}


		private BoolWrapper solarSystPanelIsClosed = new BoolWrapper();

		/// <summary>
		/// Creates top bar with info about current SolarSystem and players materials
		/// </summary>
		private void CreateSolarSystemPanel() {
			if (!solarSystPanelIsClosed.Value) {
				return;
			}
			solarSystPanelIsClosed.Value = false;
			var panel = CreatePopUpPanel();
			panel.Padding = new Thickness(5, 5, 0, 0);

			// Title label
			var label = new Label() {
				Size = new Size(panel.Width / 2, textHeight + 1),
				Text = " Select solar system:",
				Location = new Point(panel.Width / 4, 0),
				TextStyle = {
					Alignment = Miyagi.Common.Alignment.TopCenter
				}
			};
			panel.Controls.Add(label);

			List<string> solarSystList = Game.GroupManager.GetAllSolarSystemNames();

			int marginTop = 5 + label.Height;
			var innerScrollablePanel = CreateInnerScrollablePanel(marginTop, panel.Width - 28, panel.Height / 2 - (label.Height * 2));

			panel.Controls.Add(innerScrollablePanel);

			innerScrollablePanel.Padding = new Thickness(10, 5, 0, 0);

			int selectionLabelMarginTop = textHeight + 1;
			int selectionLabelWidth = innerScrollablePanel.Width;

			// Labels with Name of solar systems
			for (int i = 0; i < solarSystList.Count; i++) {
				var selectionLabel = CreateSelectionLabel(
					selectionLabelWidth, textHeight, solarSystList[i],
					new Point(0, i * selectionLabelMarginTop), i, panel,
					solarSystPanelIsClosed
					);

				innerScrollablePanel.Controls.Add(selectionLabel);
				selectionLabel.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(SelectSolarSystem);
			}

			marginTop = panel.Height / 2;

			// Title label
			label = new Label() {
				Size = new Size(panel.Width / 2, textHeight + 1),
				Text = " Travelers:",
				Location = new Point(panel.Width / 4, marginTop),
				TextStyle = {
					Alignment = Miyagi.Common.Alignment.TopCenter
				}
			};
			panel.Controls.Add(label);

			marginTop += panel.Height / 10;
			innerScrollablePanel = CreateInnerScrollablePanel(marginTop, panel.Width - 28, panel.Height / 4);
			panel.Controls.Add(innerScrollablePanel);

			innerScrollablePanel.Padding = new Thickness(10, 5, 0, 0);

			var travList = Game.GroupManager.GetTravelers();

			for (int i = 0; i < travList.Count; i++) {
				// Labels with Name of solar systems
				var testButton = CreatePropertyLabel<TimeSpan>(selectionLabelWidth, 25, travList[i].ToString(),
					new Point(0, i * selectionLabelMarginTop), travList[i].TimeProperty);
				innerScrollablePanel.Controls.Add(testButton);
			}


			var closeButton = CreateCloseButton(panel.Width / 2,
				panel.Height / 12,
				"	Cancel",
				new Point(panel.Width / 2, panel.Height * 9 / 10),
					panel, solarSystPanelIsClosed);
			closeButton.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(DisposePanel);
			panel.Controls.Add(closeButton);
			gui.Controls.Add(panel);
		}

		/// <summary>
		/// Create panel width scroll bars, setted margin-left = 10 and border thickness = 2
		/// </summary>
		/// <param Name="marginTop">Margin from top</param>
		/// <param Name="width">Width of panel</param>
		/// <param Name="height">Height of panel</param>
		/// <returns></returns>
		private Panel CreateInnerScrollablePanel(int marginTop, int width, int height) {
			return new Panel() {
				Location = new Point(10, marginTop),
				Size = new Size(width, height),
				ResizeMode = ResizeModes.None,
				Skin = skinDict["PanelSkin"],
				BorderStyle = new BorderStyle { Thickness = new Thickness(2) },
				HScrollBarStyle = new ScrollBarStyle() {
					Extent = 10,
					ThumbStyle = {
						BorderStyle = {
							Thickness = new Thickness(2, 2, 2, 2)
						}
					}
				},
				VScrollBarStyle = new ScrollBarStyle {
					Extent = 12,
					AlwaysVisible = true,
					ShowButtons = true,
					ThumbStyle = {
						BorderStyle = {
							Thickness = new Thickness(2, 2, 2, 2)
						}
					}
				}
			};
		}

		/// <summary>
		/// Create SelectionLabel - on click Select positon and can close setted panel
		/// </summary>
		/// <param Name="width">Width of Label</param>
		/// <param Name="height">Height of Label</param>
		/// <param Name="text">Text in Label</param>
		/// <param Name="location">Relative position in Panel</param>
		/// <param Name="order">Number of choice</param>
		/// <param Name="panelToClose">Closing Panel</param>
		/// <returns></returns>
		private SelectionLabel CreateSelectionLabel(int width, int height, string text, Point location, int order, Panel panelToClose, BoolWrapper isClosed,
			object objectRef = null) {
			var selectLabel = new SelectionLabel(order, objectRef, panelToClose, isClosed) {
				Size = new Size(width, height),
				Text = text,
				Location = location

			};
			return selectLabel;
		}

		/// <summary>
		/// Create SelectionLabel - on click Select positon and can close setted panel
		/// </summary>
		/// <param Name="width">Width of Label</param>
		/// <param Name="height">Height of Label</param>
		/// <param Name="text">Text in Label</param>
		/// <param Name="location">Relative position in Panel</param>
		/// <param Name="order">Number of choice</param>
		/// <param Name="panelToClose">Closing Panel</param>
		/// <returns></returns>
		private SelectionLabel CreateSelectionLabel(int width, int height, string text, Point location, int order, Panel panelToClose,
			object objectRef = null) {
			var selectLabel = new SelectionLabel(order, objectRef, panelToClose) {
				Size = new Size(width, height),
				Text = text,
				Location = location

			};
			return selectLabel;
		}

		/// <summary>
		/// Strange Name for runtime generic calling (unique Name)
		/// </summary>
		/// <typeparam Name="T">Generic type</typeparam>
		/// <param Name="marginLeft">Left indent </param>
		/// <param Name="marginTop">Top indent</param>
		/// <param Name="width">Width of Label</param>
		/// <param Name="height">Height of Label</param>
		/// <param Name="property">Property with Label's value</param>
		/// <returns>PropertyLabel as Label </returns>
		private Label CreatePropertyLabelAsLabel<T>(int marginLeft, int marginTop, int width, int height, Property<T> property) {

			var propLabel = new PropertyLabel<T>(property, "") {
				Size = new Size(width, height),
				Location = new Point(marginLeft, marginTop)
			};
			return propLabel;
		}

		private PropertyLabel<T> CreatePropertyLabel<T>(int width, int height, string text, Point location, Property<T> timeProperty) {
			var propLabel = new PropertyLabel<T>(timeProperty, text) {
				Size = new Size(width, height),
				Location = location

			};
			return propLabel;
		}

		public PictureBox CreateGameActionIcon(IGameAction action) {
			var icon = new GameActionIconBox(action);
			icon.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(GameActionClicked);
			return icon;
		}


		/// <summary>
		/// Creates classic Miyagi Button
		/// </summary>
		/// <param Name="width">Width of Button</param>
		/// <param Name="height">Hidth of Button</param>
		/// <param Name="text">Button's text</param>
		/// <param Name="location">Relative position</param>
		/// <returns>Miyagi Button</returns>
		private Button CreateButton(int width, int height, string text, Point location) {
			Button b = new Button() {
				Size = new Size(width, height),
				Location = location,
				Skin = skinDict["Button"],
				Text = text
			};
			return b;
		}

		private Label CreateLabel(int marginTop, int width, int height, string text) {
			return CreateLabel(10, marginTop, width, height, text);
		}

		private Label CreateLabel(int marginLeft, int marginTop, int width, int height, string text) {
			var label = new Label() {
				Text = text,
				Size = new Size(width, height),
				Location = new Point(marginLeft, marginTop)
			};
			return label;
		}

		private Panel CreatePopUpPanel() {
			// Background panel
			var panel = new Panel() {
				Width = screenWidth / 2,
				Height = screenHeight * 4 / 7,
				Location = new Point(screenWidth / 4, screenHeight / 5),
				Skin = skinDict["PanelR"],
				ResizeMode = ResizeModes.None
			};
			return panel;
		}

		private CloseButton CreateCloseButton(int width, int height, string text, Point location, Panel panelToClose, BoolWrapper isClosed) {
			var b = new CloseButton(panelToClose, isClosed) {
				Size = new Size(width, height),
				Location = location,
				Skin = skinDict["Button"],
				Text = text
			};
			return b;
		}

		private CloseButton CreateCloseButton(int width, int height, string text, Point location, Panel panelToClose) {
			var b = new CloseButton(panelToClose) {
				Size = new Size(width, height),
				Location = location,
				Skin = skinDict["Button"],
				Text = text
			};
			return b;
		}

		/// <summary>
		/// Dispose GUI system
		/// </summary>
		public void Dispose() {
			system.Dispose();
		}

		/// <summary>
		/// Function updete GUI
		/// </summary>
		public void Update() {
			system.Update();
		}


		public void ShowSolarSystSelectionPanel(List<string> possibilities, string topic, object gameObject) {
			var panel = CreatePopUpPanel();
			gui.Controls.Add(panel);

			int marginTop = 10;


			// Title label
			var label = new Label() {
				Size = new Size(panel.Width / 2, panel.Height / 10),
				Text = " " + topic,
				Location = new Point(panel.Width / 4, marginTop),
				TextStyle = {
					Alignment = Miyagi.Common.Alignment.TopCenter
				}
			};
			panel.Controls.Add(label);

			marginTop += label.Height;

			var innerScrollablePanel = CreateInnerScrollablePanel(marginTop, panel.Width * 30 / 31, panel.Height - (marginTop * 2));

			panel.Controls.Add(innerScrollablePanel);

			innerScrollablePanel.Padding = new Thickness(10, 5, 0, 0);

			int selectionLabelMarginTop = 26;
			int selectionLabelWidth = innerScrollablePanel.Width;

			// Create SelectionLabels with names of SolarSystems
			for (int i = 0; i < possibilities.Count; i++) {

				SelectionLabel selectionLabel = CreateSelectionLabel(
					selectionLabelWidth, 25, possibilities[i],
					new Point(0, i * selectionLabelMarginTop), i, panel, gameObject
					);

				innerScrollablePanel.Controls.Add(selectionLabel);
				selectionLabel.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(Travel);
			}

			var closeButton = CreateCloseButton(panel.Width / 2,
				panel.Height / 12,
				"	Cancel",
				new Point(panel.Width / 2, panel.Height * 9 / 10),
					panel);
			closeButton.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(DisposePanel);
			panel.Controls.Add(closeButton);

		}


		public void PrintToGameConsole(string text) {
			var label = new Label() {

				Text = text,
				Size = new Size(console.Width * 2, textHeight),
				Location = new Point(5, consoleLinesNumber * (textHeight + 1))
			};
			consoleLinesNumber++;
			console.Controls.Add(label);
			console.ScrollToBottom();
		}

		public void ShowIGameActionIcons(List<IGameAction> actionList) {

			var maxLineWidth = gameActionPanel.Width - 25; // Padding and Scroll bar fix
			var actualLineWidth = 0;
			var lineCount = 0;

			foreach (var action in actionList) {
				var icon = CreateGameActionIcon(action);
				icon.Location = new Point(actualLineWidth, lineCount * 30);
				actualLineWidth += 27;
				if (actualLineWidth > maxLineWidth) {
					lineCount++;
					actualLineWidth = 0;
				}
				gameActionPanel.Controls.Add(icon);
			}


		}

		///
		///
		///pokusy
		///

		//public void SetMaterialState(string material, int inc) {
		//	if (materialList.ContainsKey(material)) {
		//		materialList[material].value.Text = (inc).ToString();
		//	} else {
		//		throw new Strategy.Exceptions.MissingMaterialException("This Material is not in your list. You can not set value to nonexist material.");
		//	}
		//}
	}
}
