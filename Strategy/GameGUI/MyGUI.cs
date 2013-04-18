using System;
using System.Collections.Generic;
using Miyagi.Common.Data;
using Miyagi.Common.Resources;
using Miyagi.Common;
using Miyagi.UI;
using Miyagi.UI.Controls;
using Miyagi.UI.Controls.Styles;
using Strategy.GameMaterial;
using Strategy.GameObjectControl;
using Strategy.GameObjectControl.RuntimeProperty;
using System.Reflection;

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

		private Label statPanelName;
		private Panel propertyPanel;

		Dictionary<string, Skin> skinDict;
		Dictionary<string, Font> fonts;
		MiyagiSystem system;
		GUI gui;

		private const string nothingSelected = "	Nothing selected";

		protected Dictionary<string, MaterialGUIPair> materialList;

		/// <summary>
		/// Constructor initializes GUI systrem for Mogre and load fonts, skins and create control panel.
		/// Also the constuctor creates mouse bounds (app using Miagi cursor for control)
		/// </summary>
		/// <param name="width">Window width</param>
		/// <param name="height">Window height</param>
		/// <param name="mouse">Mogre mouse input</param>
		/// <param name="keyboard">Mogre keyboard input</param>
		/// <param name="materials">Names and values of user materials</param>
		/// <param name="groupMgr">GroupManager instance</param>
		public MyGUI(int width, int height, MOIS.Mouse mouse, MOIS.Keyboard keyboard, Dictionary<string, IMaterial> materials) {

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

			createMainMenu();
			createTopMenu(materials);

			createCameraBounds();				// Create bounds around screen to move the camera
		}

		/// <summary>
		/// Function creates invisible bounds for moving with camera
		/// </summary>
		private void createCameraBounds() {

			// Top bound
			var topSection = new Button() {
				Size = new Size(screenWidth * 13 / 15, screenHeight / 10),
				Location = new Point(screenWidth / 15, screenHeight * 2 / 30),
				Name = "topSection"
			};
			topSection.MouseHover += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseOver);
			topSection.MouseLeave += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseLeave);
			gui.Controls.Add(topSection);

			// Botton bound
			var backSection = new Button() {
				Size = new Size(screenWidth * 13 / 15, screenHeight / 10),
				Location = new Point(screenWidth / 15, screenHeight * 7 / 10),
				Name = "backSection"
			};
			backSection.MouseHover += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseOver);
			backSection.MouseLeave += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseLeave);
			gui.Controls.Add(backSection);

			// Left bound
			var leftSection = new Button() {
				Size = new Size(screenWidth / 15, screenHeight - (screenHeight * 4 / 10)),
				Location = new Point(0, screenHeight / 10),
				Name = "leftSection"
			};
			leftSection.MouseHover += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseOver);
			leftSection.MouseLeave += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseLeave);
			gui.Controls.Add(leftSection);

			// Right bound
			var rightSection = new Button() {
				Size = new Size(screenWidth / 15, screenHeight - (screenHeight * 4 / 10)),
				Location = new Point(screenWidth * 14 / 15, screenHeight / 10),
				Name = "rightSection"
			};
			rightSection.MouseHover += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseOver);
			rightSection.MouseLeave += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseLeave);
			gui.Controls.Add(rightSection);

			// Rigth-up corner bound
			var rightUpSection = new Button() {
				Size = new Size(screenWidth / 15, screenHeight / 10),
				Location = new Point(screenWidth * 14 / 15, 0),
				Name = "rightUpSection"
			};
			rightUpSection.MouseHover += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseOver);
			rightUpSection.MouseLeave += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseLeave);
			gui.Controls.Add(rightUpSection);

			// Right-down corner bound
			var rightDownSection = new Button() {
				Size = new Size(screenWidth / 15, screenHeight / 10),
				Location = new Point(screenWidth * 14 / 15, screenHeight * 7 / 10),
				Name = "rightDownSection"
			};
			rightDownSection.MouseHover += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseOver);
			rightDownSection.MouseLeave += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseLeave);
			gui.Controls.Add(rightDownSection);

			// Left-down corner bound
			var leftDownSection = new Button() {
				Size = new Size(screenWidth / 15, screenHeight / 10),
				Location = new Point(0, screenHeight * 7 / 10),
				Name = "leftDownSection"
			};
			leftDownSection.MouseHover += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseOver);
			leftDownSection.MouseLeave += new EventHandler<Miyagi.Common.Events.MouseEventArgs>(mouseLeave);
			gui.Controls.Add(leftDownSection);

			// Left-up corner bound
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
		/// Change printed solar system name
		/// </summary>
		/// <param name="name">New printed SolarSystem's name</param>
		public void setSolarSystemName(string name) {
			nameOfSolarSystem.Text = name;
		}

		/// <summary>
		/// Function shows every given Property from in propertisDict in propertyPanel
		/// </summary>
		/// <param name="propertiesDict">Dictionary with properties (key - name, value - Property)</param>
		private void showGroupProperties(Dictionary<string, object> propertiesDict) {
			var propDict = propertiesDict;
			int marginLeft = propertyPanel.Width / 2;
			int marginTop = 26;
			int i = 0;

			// Add each property int propertyPanel name and value in one row
			foreach (var property in propDict) {				
				propertyPanel.Controls.Add(createLabel(marginTop * i, marginLeft, 26, property.Key));

				propertyPanel.Controls.Add((Label)createPropertyLabelAsObject(marginLeft, marginTop * i, property.Value));
				++i;
			}
		}


		/// <summary>
		/// Function shows Group's property and count or name of object (if is just one in the group)
		/// </summary>
		/// <param name="group">Showing group with IMovableGameObjects</param>
		public void showTargeted(GameObjectControl.GroupMovables group) {
			clearStatPanelProp();

			if (group.Count == 1) { // Just one object
				statPanelName.Text = group[0].Name;
			} else {
				statPanelName.Text = "Count: " + group.Count;
			}
			showGroupProperties(group.getPropertyToDisplay());

		}


		/// <summary>
		/// Function shows Group's property and count or name of object (if is just one in the group)
		/// Also can show nothingSelect constant if group is empty
		/// </summary>
		/// <param name="group"></param>
		public void showTargeted(GameObjectControl.GroupStatics group) {
			 
			clearStatPanelProp();
			if (group == null || group.Count == 0) {
				statPanelName.Text = nothingSelected;
				propertyPanel.Controls.Add(new Label() {
					Text = nothingSelected,
					Location = new Point(0, 0),
					Size = new Size(propertyPanel.Width, propertyPanel.Height)
				});
				return;
			}

			showGroupProperties(group.getPropertyToDisplay());

			if (group.Count == 1) { // Just one object
				var isgo = group[0];
				statPanelName.Text = isgo.Name;


			} else {// Player's planets (only way when is selected more than 1 IStaticGameObjects -> display actions
				statPanelName.Text = "Count: "+ group.Count;




			}

		}

		/// <summary>
		/// Creeates generic PropertyLabel with runtime setted type
		/// </summary>
		/// <param name="marginLeft">Left indent</param>
		/// <param name="marginTop">Top indent</param>
		/// <param name="property">Property </param>
		/// <returns>PropertyLabel as object</returns>
		private object createPropertyLabelAsObject(int marginLeft, int marginTop, object property) {
			MethodInfo method = typeof(MyGUI).GetMethod("createPropertyLabelAsLabel", BindingFlags.NonPublic | BindingFlags.Instance); //createPropertyLabelAsLabel is private function
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
		/// <param name="materials">Players materials</param>
		private void createTopMenu(Dictionary<string, IMaterial> materials) {
			upperMenu = new FlowLayoutPanel() {				//create top panel (empty blue box)
				Size = new Size(screenWidth * 18 / 20, screenHeight * 2 / 30),
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

			nameOfSolarSystem = new Label() {				//Label to show actual solar system name
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

			//Material load - create one line with name of IMaterial and value
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
			foreach (KeyValuePair<string, IMaterial> k in materials) {			//creates pair name - value
				materialList.Add(k.Key, new MaterialGUIPair(k.Key, k.Value.state, materialBox.Width, row));
				row++;
			}

			row = 0;
			foreach (KeyValuePair<string, MaterialGUIPair> valuePair in materialList) {		//set pairs into GUI
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
			b = createButton(buttonsPanel.Width / 2, buttonsPanel.Height / 5, "  Solar systems", new Point(buttonMarginLeft, buttonMarginTop));
			buttonsPanel.Controls.Add(b);
			b.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(showSolarSystems);

			b = createButton(buttonsPanel.Width / 2, buttonsPanel.Height / 5, "  Exit BUTTON", new Point(buttonMarginLeft, buttonMarginTop * 2));
			buttonsPanel.Controls.Add(b);
			b.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(quitOnClick);


			return buttonsPanel;
		}

		/// <summary>
		/// Create second panel in main panel
		/// </summary>
		/// <returns>Miyagi Panel</returns>
		private Panel createStatPanel() {
			statPanel = createMainmenuSubpanel();
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

			propertyPanel = createInnerScrollablePanel(y, statPanel.Width - 30, statPanel.Height - (y+10)); //minus 30 is padding correction (2*5 panel padding + 2*10 scrollablePanel padding)
			statPanel.Controls.Add(propertyPanel);

			return statPanel;

		}

		/// <summary>
		/// Clear panel with descriptions
		/// </summary>
		private void clearStatPanelProp() {
			propertyPanel.Controls.Clear();
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


		#endregion

		#region Button Actions
		private void selectSolarSystem(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			// Button Action for SelectionLabel calls GroupManager function changeSolarSystem and close Panel
			if (sender.GetType() == typeof(SelectionLabel)) {
				GroupManager.getInstance().changeSolarSystem(((SelectionLabel)sender).NumberOfItem);
				((SelectionLabel)sender).PanelToClose.Dispose();
			}
		}

		private void travel(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			// Function calls createTraveler with selected number of SolarSystem and traveler(IMovableGameObject)
			if (sender.GetType() == typeof(SelectionLabel)) {
				var selLabel = (SelectionLabel)sender;
				GroupManager.getInstance().createTraveler(selLabel.NumberOfItem, selLabel.StoredObject);
				selLabel.PanelToClose.Dispose();
			}
		}

		private void quitOnClick(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			// Function closes application (throw ShutdownException)
			system.Dispose();
			throw new Strategy.Exceptions.ShutdownException();
		}

		private void pause(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			Game.pause(!Game.GameStatus);
		}

		private void mouseOver(object sender, Miyagi.Common.Events.MouseEventArgs e) {
			// Controls camera move
			MogreControl.MouseControl.move(((Button)sender).Name);

		}

		private void mouseLeave(object sender, Miyagi.Common.Events.MouseEventArgs e) {
			// Controls camera move
			MogreControl.MouseControl.moveStop(((Button)sender).Name);
		}

		private void disposePanel(object sender, Miyagi.Common.Events.MouseEventArgs e) {
			// On click button dispose the panel 
			if (sender.GetType() == typeof(CloseButton)) {
				((CloseButton)sender).PanelToClose.Dispose();
			}
		}

		private void showSolarSystems(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			// Shows Panel with SolarSystems and travelers
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

			var panel = createPopUpPanel();

			int marginTop = 10;

			// Title label
			var label = new Label() {								
				Size = new Size(panel.Width / 2, panel.Height / 10),
				Text = " Select solar system:",
				Location = new Point(panel.Width / 4, marginTop),
				TextStyle = {
					Alignment = Miyagi.Common.Alignment.TopCenter
				}
			};
			panel.Controls.Add(label);

			List<string> solarSystList = GroupManager.getInstance().getAllSolarSystemNames();

			marginTop += 5 + label.Height;
			var innerScrollablePanel = createInnerScrollablePanel(marginTop, panel.Width * 30 / 31, panel.Height / 3);

			panel.Controls.Add(innerScrollablePanel);

			int selectionLabelMarginTop = 26;
			int selectionLabelMarginLeft = innerScrollablePanel.Width / 8;
			int selectionLabelWidth = innerScrollablePanel.Width;

			// Labels with name of solar systems
			for (int i = 0; i < solarSystList.Count; i++) {					
				var selectionLabel = createSelectionLabel(
					selectionLabelWidth, 25, solarSystList[i],
					new Point(selectionLabelMarginLeft, i * selectionLabelMarginTop), i, panel
					);

				innerScrollablePanel.Controls.Add(selectionLabel);
				selectionLabel.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(selectSolarSystem);
			}

			marginTop = panel.Height / 2;

			// Title label
			label = new Label() {											
				Size = new Size(panel.Width / 2, panel.Height / 10),
				Text = " Travelers:",
				Location = new Point(panel.Width / 4, marginTop),
				TextStyle = {
					Alignment = Miyagi.Common.Alignment.TopCenter
				}
			};
			panel.Controls.Add(label);

			marginTop += panel.Height / 10;
			innerScrollablePanel = createInnerScrollablePanel(marginTop, panel.Width * 30 / 31, panel.Height / 4);
			panel.Controls.Add(innerScrollablePanel);

			var travList = GroupManager.getInstance().getTravelers();



			for (int i = 0; i < travList.Count; i++) {
				// Labels with name of solar systems
				var testButton = createPropertyLabel<TimeSpan>(selectionLabelWidth, 25, travList[i].ToString(),
					new Point(selectionLabelMarginLeft, i * selectionLabelMarginTop), travList[i].TimeProperty);
				innerScrollablePanel.Controls.Add(testButton);
			}


			var closeButton = createCloseButton(panel.Width / 2,
				panel.Height / 12,
				"	Cancel",
				new Point(panel.Width / 2, panel.Height * 9 / 10),
					panel);
			closeButton.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(disposePanel);
			panel.Controls.Add(closeButton);
			gui.Controls.Add(panel);
		}

		/// <summary>
		/// Create panel width scroll bars, setted margin-left = 10 and border thickness = 2
		/// </summary>
		/// <param name="marginTop">Margin from top</param>
		/// <param name="width">Width of panel</param>
		/// <param name="height">Height of panel</param>
		/// <returns></returns>
		private Panel createInnerScrollablePanel(int marginTop, int width, int height) {
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
		/// Create SelectionLabel - on click select positon and can close setted panel
		/// </summary>
		/// <param name="width">Width of Label</param>
		/// <param name="height">Height of Label</param>
		/// <param name="text">Text in Label</param>
		/// <param name="location">Relative position in Panel</param>
		/// <param name="order">Number of choice</param>
		/// <param name="panelToClose">Closing Panel</param>
		/// <returns></returns>
		private SelectionLabel createSelectionLabel(int width, int height, string text, Point location, int order, Panel panelToClose, object objectRef = null) {
			var selectLabel = new SelectionLabel(order, objectRef, panelToClose) {
				Size = new Size(width, height),
				Text = text,
				Location = location

			};
			return selectLabel;
		}

		/// <summary>
		/// Strange name for runtime generic calling (unique name)
		/// </summary>
		/// <typeparam name="T">Generic type</typeparam>
		/// <param name="marginLeft">Left indent </param>
		/// <param name="marginTop">Top indent</param>
		/// <param name="width">Width of Label</param>
		/// <param name="height">Height of Label</param>
		/// <param name="property">Property with Label's value</param>
		/// <returns>PropertyLabel as Label </returns>
		private Label createPropertyLabelAsLabel<T>(int marginLeft, int marginTop, int width, int height, Property<T> property) {

			var propLabel = new PropertyLabel<T>(property, "") {
				Size = new Size(width, height),
				Location = new Point(marginLeft, marginTop)
			};
			return propLabel;
		}

		private PropertyLabel<T> createPropertyLabel<T>(int width, int height, string text, Point location, Property<T> timeProperty) {
			var propLabel = new PropertyLabel<T>(timeProperty, text) {
				Size = new Size(width, height),
				Location = location

			};
			return propLabel;
		}

		/// <summary>
		/// Creates classic Miyagi Button
		/// </summary>
		/// <param name="width">Width of Button</param>
		/// <param name="height">Hidth of Button</param>
		/// <param name="text">Button's text</param>
		/// <param name="location">Relative position</param>
		/// <returns>Miyagi Button</returns>
		private Button createButton(int width, int height, string text, Point location) {
			Button b = new Button() {
				Size = new Size(width, height),
				Location = location,
				Skin = skinDict["Button"],
				Text = text
			};
			return b;
		}

		private Label createLabel(int marginTop, int width, int height, string text) {
			return createLabel(10, marginTop, width, height, text);
		}

		private Label createLabel(int marginLeft, int marginTop, int width, int height, string text) {
			var label = new Label() {
				Text = text,
				Size = new Size(width, height),
				Location = new Point(marginLeft, marginTop)
			};
			return label;
		}

		private Panel createPopUpPanel() {
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


		public void showSolarSystSelectionPanel(List<string> possibilities, string topic, object gameObject) {
			var panel = createPopUpPanel();
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

			var innerScrollablePanel = createInnerScrollablePanel(marginTop, panel.Width * 30 / 31, panel.Height - (marginTop * 2));

			panel.Controls.Add(innerScrollablePanel);

			int selectionLabelMarginTop = 26;
			int selectionLabelMarginLeft = innerScrollablePanel.Width / 8;
			int selectionLabelWidth = innerScrollablePanel.Width;


			// Create SelectionLabels with names of SolarSystems
			for (int i = 0; i < possibilities.Count; i++) {
				SelectionLabel selectionLabel = createSelectionLabel(
					selectionLabelWidth, 25, possibilities[i],
					new Point(selectionLabelMarginLeft, i * selectionLabelMarginTop), i, panel, gameObject
					);

				innerScrollablePanel.Controls.Add(selectionLabel);
				selectionLabel.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(travel);
			}

			var closeButton = createCloseButton(panel.Width / 2,
				panel.Height / 12,
				"	Cancel",
				new Point(panel.Width / 2, panel.Height * 9 / 10),
					panel);
			closeButton.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(disposePanel);
			panel.Controls.Add(closeButton);

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
