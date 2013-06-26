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
using System.IO;
using Strategy.Exceptions;

namespace Strategy.GameGUI {

	public enum PanelType { MissionInfoPanel, LoadPanel, SavePanel, SolarSystemPanel, TravelPanel, MissionEndPanel }


	public class MyGUI : IGameGUI {

		Dictionary<string, Panel> openedPanelDict;

		// Screen parameters
		protected int screenWidth;
		protected int screenHeight;

		// Menu panels
		protected FlowLayoutPanel upperMenu;
		protected FlowLayoutPanel mainMenu;

		protected Label nameOfSolarSystem;

		// Mainmenu sections
		private Panel buttonsPanel;
		private Panel statPanel;
		private Panel actionPanel;

		private bool enable;

		// Game buttons
		private Button pauseButton;
		private Button loadButton;
		private Button saveButton;
		private Button missionButton;
		private Button solarSystemButton;

		// Targeted info
		private Label statPanelName;

		// Mission info panels
		private Panel propertyPanel;
		private Panel gameActionPanel;
		private Panel materialPanel;

		// Game console
		private Panel console;
		private int consoleLinesNumber;

		Dictionary<string, Skin> skinDict;
		Dictionary<string, Font> fonts;
		MiyagiSystem guiSystem;
		GUI gui;

		private const string nothingSelected = "Nothing selected";
		private const int textHeight = 18;


		/// <summary>
		/// Initializes GUI systrem for Mogre and Load fonts, skins and creates GUI overlay.
		/// </summary>
		/// <param Name="width">The window width</param>
		/// <param Name="height">The window height</param>
		/// <param Name="mouse">The Mogre mouse input</param>
		/// <param Name="keyboard">The mogre keyboard input</param>
		public MyGUI(int width, int height, MOIS.Mouse mouse, MOIS.Keyboard keyboard) {
			screenHeight = height;
			screenWidth = width;

			openedPanelDict = new Dictionary<string, Panel>();

			// Set Miyagi to Mogre
			guiSystem = new MiyagiSystem("Mogre");
			gui = new GUI();
			guiSystem.GUIManager.GUIs.Add(gui);
			guiSystem.PluginManager.LoadPlugin(@"Miyagi.Plugin.Input.Mois.dll", mouse, keyboard);


			// Font loading from xml file
			fonts = new Dictionary<string, Font>();
			foreach (Font font in TrueTypeFont.CreateFromXml("../../Media/TrueTypeFonts.xml", guiSystem))
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
			Cursor cursor = new Cursor(cursorSkin, new Size(15, 15), new Point(0, 0), true);

			// Set cursor
			guiSystem.GUIManager.Cursor = cursor;

			// Create panels
			CreateMainMenu();
			CreateTopMenu();
		}

		#region Public

		/// <summary>
		/// Shows the group properties and count or name of the object (if is just one in the group).
		/// </summary>
		/// <param Name="group">The showing group with IMovableGameObjects</param>
		public void ShowTargeted(GroupMovables group) {
			ClearGroupPanels();

			if (group.Count == 1) { // Just one object
				statPanelName.Text = group[0].Name;
			} else {
				statPanelName.Text = "Count: " + group.Count;
			}

			ShowGroupProperties(group.GetPropertyToDisplay());

			if (group.Team.Name == Game.PlayerName) {
				ShowIGameActionIcons(group.GetGroupIGameActions());
			}
		}


		/// <summary>
		/// Shows group properties and count or name of the object (if is just one in the group).
		/// Also can show "nothingSelect" constant if group is empty.
		/// When it is player group so shows gameActions icons.
		/// </summary>
		/// <param Name="group">The showing group with IStaticGameObjects</param>
		public void ShowTargeted(GroupStatics group) {

			ClearGroupPanels();

			// Empty group
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

			if (group.Team.Name == Game.PlayerName) {
				ShowIGameActionIcons(group.GetGroupIGameActions());
			}

			if (group.Count == 1) {
				// Just one object in the group.
				var isgo = group[0];
				statPanelName.Text = isgo.Name;
			} else {
				// More objects -> show count
				statPanelName.Text = "Count: " + group.Count;
			}
		}

		/// <summary>
		/// Prints the name as actual solar system name.
		/// </summary>
		/// <param Name="Name">The SolarSystem Name</param>
		public void SetSolarSystemName(string name) {
			nameOfSolarSystem.Text = name;
		}

		/// <summary>
		/// Clears all GUI object which contains a mission data.
		/// </summary>
		public void ClearMissionData() {
			nameOfSolarSystem.Text = "Booted system";
			materialPanel.Controls.Clear();
			console.Controls.Clear();
			consoleLinesNumber = 0;
		}

		/// <summary>
		/// Disposes panel by the name.
		/// </summary>
		/// <param name="panelName">The name of the given panel.</param>
		public void ClosePanel(string panelName) {
			if (openedPanelDict.ContainsKey(panelName)) {
				openedPanelDict[panelName].Dispose();
				openedPanelDict.Remove(panelName);
			}
		}

		/// <summary>
		/// Returns number of the opened panels.
		/// </summary>
		public int NumberOfPopUpPanels {
			get { return openedPanelDict.Count; }
		}

		/// <summary>
		/// Disposes GUI system.
		/// </summary>
		public void Dispose() {
			guiSystem.Dispose();
		}

		/// <summary>
		/// Updetes GUI system.
		/// </summary>
		public void Update() {
			guiSystem.Update();
		}

		/// <summary>
		/// Creates PopUpPanel with SelectionLabels (names of the destiantions). SelectionLabels 
		/// are closing the panel and send information to create traveler on MouseClick. 
		/// </summary>
		/// <param name="possibilities">The list with names of the destintions.</param>
		/// <param name="gameObject">The potential traveler.</param>
		public void ShowTravelSelectionPanel(List<string> possibilities, object gameObject) {
			// Creates PopUpPanel (this is not singleton).
			var panel = CreatePopUpPanel("Choose treval destiantion:", PanelType.TravelPanel, false);
			gui.Controls.Add(panel);

			int marginTop = 11 + textHeight;
			// Creates ScrollablePanel to show destinations.
			var scrollablePanel = CreateScrollablePanel(marginTop, panel.Width - 28, panel.Height * 37 / 48);
			panel.Controls.Add(scrollablePanel);

			int selectionLabelMarginTop = 26;
			int selectionLabelWidth = scrollablePanel.Width;

			// Create SelectionLabels with names of the SolarSystems
			for (int i = 0; i < possibilities.Count; i++) {
				SelectionLabel selectionLabel = CreateSelectionLabel(
					selectionLabelWidth, 25, possibilities[i],
					new Point(0, i * selectionLabelMarginTop), i, panel.Name, gameObject
					);
				scrollablePanel.Controls.Add(selectionLabel);
			}
		}

		/// <summary>
		/// Prints given text to the game console (ScrollablePanel). Text is printed on a new line
		/// and console is scroll to bottom.
		/// </summary>
		/// <param name="text">The text to print.</param>
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

		/// <summary>
		/// Loads icons (GameActionIconBox) for each IGameAction in given List and insert them to gameActionPanel.
		/// </summary>
		/// <param name="actionList">The list that contains IGameActions to display.</param>
		public void ShowIGameActionIcons(List<IGameAction> actionList) {
			// Padding and Scroll bar fix
			var maxLineWidth = gameActionPanel.Width - 25;
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

		/// <summary>
		/// Reloads materialPanel (adds names and values for each given material).
		/// </summary>
		/// <param name="materialDict">The dictionary that contains IMaterials to display.</param>
		public void UpdatePlayerMaterialDict(Dictionary<string, IMaterial> materialDict) {
			int row = 0;

			foreach (var materialPair in materialDict) {
				var nameLabel = CreateLabel(row * (textHeight + 1), materialPanel.Width / 3, textHeight, materialPair.Key);
				var valueLabel = CreatePropertyLabelGeneric<int>(materialPanel.Width / 2, row * (textHeight + 1),
					materialPanel.Width / 3, textHeight, materialPair.Value.GetQuantityOfMaterial());

				valueLabel.TextStyle = new TextStyle {
					Alignment = Miyagi.Common.Alignment.MiddleRight,
					ForegroundColour = Colours.White
				};

				materialPanel.Controls.Add(nameLabel);
				materialPanel.Controls.Add(valueLabel);
				row++;
			}
		}

		/// <summary>
		/// Gets and sets GUI enable mode. In enable mode GUI has mission buttons (Save, Mission Info, SolarSystem Pause)
		/// and game control buttons (Pause, End Mission, Exit). In disable mode just game control buttons and Load button.
		/// </summary>
		public bool Enable {
			get { return enable; }
			set {
				pauseButton.Enabled = value;
				loadButton.Visible = !value;
				saveButton.Visible = value;
				solarSystemButton.Visible = value;
				missionButton.Visible = value;
				enable = value;
			}
		}

		/// <summary>
		/// Shows mission end panel with information about reason why game ends and clears all mission data
		/// (disposes or clears panel with mission data).
		/// </summary>
		/// <param name="printText">The reason why game ends.</param>
		public void MissionEnd(string printText) {
			// Checks if panel is showed (is singleton)
			if (openedPanelDict.ContainsKey(PanelType.MissionEndPanel.ToString())) {
				return;
			}

			// Switchs to disable mode and clear mission data.
			Game.Paused = true;
			ClearMissionData();
			DisableAllMissionControlButtons();

			var panel = new Panel() {
				Size = new Size(screenWidth / 4, screenHeight / 5),
				Location = new Point(screenWidth * 3 / 8, screenHeight * 2 / 5),
				Text = "Game ends. " + printText,
				Skin = skinDict["PanelR"],
				ResizeMode = ResizeModes.None,
				TextStyle = new TextStyle {
					Alignment = Alignment.MiddleCenter
				}
			};
			// Registers panel - singleton variant
			openedPanelDict.Add(PanelType.MissionEndPanel.ToString(), panel);

			// Creates close button
			var button = CreateCloseButton(panel.Width / 4, panel.Height / 6, "Ok",
				new Point(panel.Width * 3 / 8, panel.Height * 3 / 4), PanelType.MissionEndPanel.ToString());
			button.MouseClick += EndMission;
			panel.Controls.Add(button);

			gui.Controls.Add(panel);
		}
		#endregion

		#region Top panel
		/// <summary>
		/// Creates top bar with Label for actual name of the SolarSystem, ScrollablePanel for materials states.
		/// </summary>
		private void CreateTopMenu() {
			// Create top panel
			upperMenu = new FlowLayoutPanel() {
				Size = new Size(screenWidth * textHeight / 20, screenHeight / 14),
				Location = new Point(screenWidth / 20, 0),
				Skin = skinDict["Panel"]
			};
			upperMenu.Padding = new Thickness(5);
			gui.Controls.Add(upperMenu);

			// Left Label with text "Current solar system:"
			Label label1 = new Label() {
				Name = "nameOfSolarSystem",
				Size = new Size(upperMenu.Width / 6, upperMenu.Height * 4 / 5),
				Text = "Current solar system: ",
				TextStyle = {
					Alignment = Miyagi.Common.Alignment.MiddleLeft,
				},
				Padding = new Thickness(10, 0, 0, 0)
			};
			upperMenu.Controls.Add(label1);

			// Label to show actual solar system name
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

			// Right Label with text "Material State:"
			Label materialIntro = new Label() {
				Name = "MaterialState",
				Size = new Size(upperMenu.Width / 6, upperMenu.Height * 4 / 5),
				Text = "Material State: ",
				TextStyle = {
					Alignment = Miyagi.Common.Alignment.MiddleLeft
				},
				Padding = new Thickness(1)
			};
			upperMenu.Controls.Add(materialIntro);

			// Creates ScrollablePanel for material states
			materialPanel = CreateScrollablePanel(5, upperMenu.Width * 2 / 5, upperMenu.Height - 10);
			upperMenu.Controls.Add(materialPanel);
		}
		#endregion

		#region Main panel

		/// <summary>
		/// Creates main bar and calls functions to create subpanels (CreateButton/Stat/ActionPanel()).
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
		/// Creates panel with parameters calculated from mainMenu panel.
		/// </summary>
		/// <returns>Returns panel.</returns>
		private Panel CreateMainmenuSubpanel() {
			var p = new Panel() {
				ResizeMode = ResizeModes.None,
				Padding = new Thickness(5),
				Size = new Size(mainMenu.Width / 3, mainMenu.Height),
				Skin = skinDict["PanelR"]
			};
			return p;
		}

		/// <summary>
		/// Creates mainmenu subpanel. Initializes game controls buttons (Pause, Save, Load, End Mission and Exit) 
		/// and mission controls button (Mission info, Solar systems).
		/// </summary>
		/// <returns>Returns initialized Panel with buttons.</returns>
		private Panel CreateButtonPanel() {
			buttonsPanel = CreateMainmenuSubpanel();
			int buttonMarginLeft = buttonsPanel.Width / 9;
			int buttonMarginLeftSecond = buttonsPanel.Width / 2 + buttonMarginLeft / 2;
			int buttonMarginTop = buttonsPanel.Height * 6 / 25;
			int row = 0;

			int buttonWidth = buttonsPanel.Width / 3;

			// Creates Pause button with MouseClick action Pause (disable, visible)
			pauseButton = CreateButton(buttonWidth, buttonsPanel.Height / 5, "Pause BUTTON", new Point(buttonMarginLeft, buttonMarginTop * row));
			buttonsPanel.Controls.Add(pauseButton);
			pauseButton.MouseClick += Pause;
			pauseButton.Enabled = false;
			row++;

			// Creates Save button with MouseClick action ShowSavePanel (enable, invisible)
			saveButton = CreateButton(buttonWidth, buttonsPanel.Height / 5, "Save", new Point(buttonMarginLeft, buttonMarginTop * row));
			buttonsPanel.Controls.Add(saveButton);
			saveButton.MouseClick += ShowSavePanel;
			saveButton.Visible = false;

			// Creates Load button with MouseClick action ShowLoadPanel (enable, visible)
			loadButton = CreateButton(buttonWidth, buttonsPanel.Height / 5, "Load", new Point(buttonMarginLeft, buttonMarginTop * row));
			buttonsPanel.Controls.Add(loadButton);
			loadButton.MouseClick += ShowLoadPanel;
			row++;

			// Creates End Mission button with MouseClick action EndMission (enable, visible)
			Button b = CreateButton(buttonWidth, buttonsPanel.Height / 5, "End Mission", new Point(buttonMarginLeft, buttonMarginTop * row));
			buttonsPanel.Controls.Add(b);
			b.MouseClick += EndMission;
			row++;

			// Creates Exit button (enable, visible)
			b = CreateExitButton(buttonWidth, buttonsPanel.Height / 5, new Point(buttonMarginLeft, buttonMarginTop * row));
			buttonsPanel.Controls.Add(b);

			row = 0;
			// Creates Mission Info button with MouseClick action ShowMissionInfo (enable, invisible)
			missionButton = CreateButton(buttonWidth, buttonsPanel.Height / 5, "Mission Info", new Point(buttonMarginLeftSecond, buttonMarginTop * row));
			buttonsPanel.Controls.Add(missionButton);
			missionButton.MouseClick += ShowMissionInfo;
			missionButton.Visible = false;
			row++;

			// Creates Solar Systems button with MouseClick action ShowSolarSystems (enable, invisible)
			solarSystemButton = CreateButton(buttonWidth, buttonsPanel.Height / 5, "Solar Systems", new Point(buttonMarginLeftSecond, buttonMarginTop * row));
			buttonsPanel.Controls.Add(solarSystemButton);
			solarSystemButton.MouseClick += ShowSolarSystems;
			solarSystemButton.Visible = false;
			row++;

			return buttonsPanel;
		}

		/// <summary>
		/// Creates mainmenu subpanel. Initializes statPanelName label (for dispaly targeted group name or count) 
		/// and propertyPanel for targeted group properties.
		/// </summary>
		/// <returns>Returns initialized Panel with ScrollablePanel for group properties.</returns>
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

			// Creates ScrollablePanel for targeted group properties.
			propertyPanel = CreateScrollablePanel(y, statPanel.Width - 30, statPanel.Height - (y + 10));
			statPanel.Controls.Add(propertyPanel);

			return statPanel;
		}

		/// <summary>
		/// Clears panel with group properties and with group actions
		/// </summary>
		private void ClearGroupPanels() {
			propertyPanel.Controls.Clear();
			gameActionPanel.Controls.Clear();
		}

		/// <summary>
		/// Creates mainmenu subpanel. Initializes gameActionPanel for targeted group game actions and game console.
		/// </summary>
		/// <returns>Returns initialized Panel with ScrollablePanel for group action and with game console.</returns>
		private Panel CreateActionPanel() {
			actionPanel = CreateMainmenuSubpanel();

			// Creates ScrollablePanel for group game actions.
			gameActionPanel = CreateScrollablePanel(5, actionPanel.Width - 30, actionPanel.Height / 2 - 10);
			actionPanel.Controls.Add(gameActionPanel);

			// Creates game console.
			console = CreateScrollablePanel(actionPanel.Height / 2 + 1, actionPanel.Width - 30, actionPanel.Height / 2 - 10);
			actionPanel.Controls.Add(console);

			return actionPanel;
		}
		#endregion

		#region Button Actions

		/// <summary>
		/// Calls CreateSavePanel() which displays the PopUpPanel for saving the game.
		/// </summary>
		/// <param name="sender">The action sender.</param>
		/// <param name="e">The action argument.</param>
		private void ShowSavePanel(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			CreateSavePanel();
		}

		/// <summary>
		/// Calls CreateSavePanel() which displays the PopUpPanel for loading a new game.
		/// </summary>
		/// <param name="sender">The action sender.</param>
		/// <param name="e">The action argument.</param>
		private void ShowLoadPanel(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			CreateLoadPanel();
		}

		/// <summary>
		/// Closes all opened panel which are registed in openPanelDict and calls DestroyGame (destoy current mission).
		/// </summary>
		/// <param name="sender">The action sender.</param>
		/// <param name="e">The action argument.</param>
		private void EndMission(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			foreach (var item in new Dictionary<string, Panel>(openedPanelDict)) {
				item.Value.Dispose();
				openedPanelDict.Remove(item.Key);
			}
			Game.DestroyMission();
		}

		/// <summary>
		/// Disposes GUI system and throws ShutdownException.
		/// </summary>
		/// <param name="sender">The action sender.</param>
		/// <param name="e">The action argument.</param>
		private void QuitOnClick(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			guiSystem.Dispose();
			throw new Strategy.Exceptions.ShutdownException();
		}

		/// <summary>
		/// Checks if sender is TextBox and set focus. Also captures the game keyboard (Game.KeyboardCapture).
		/// </summary>
		/// <param name="sender">The action sender.</param>
		/// <param name="e">The action argument.</param>
		private void CaptureKeyboard(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			var casted = sender as TextBox;
			if (casted != null) {
				casted.Focused = true;
			}
			Game.KeyboardCaptured = true;
		}

		/// <summary>
		/// Checks if sender is TextBox and unset focus. Also release the game keyboard (Game.KeyboardCapture).
		/// </summary>
		/// <param name="sender">The action sender.</param>
		/// <param name="e">The action argument.</param>
		private void ReleaseKeyboard(object sender, Miyagi.Common.Events.MouseEventArgs e) {
			var casted = sender as TextBox;
			if (casted != null) {
				casted.Focused = false;
			}
			Game.KeyboardCaptured = false;
		}

		/// <summary>
		/// Negates the current game status and set it (pause the running game / run the paused game).
		/// </summary>
		/// <param name="sender">The action sender.</param>
		/// <param name="e">The action argument.</param>
		private void Pause(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			Game.Paused = !Game.Paused;
		}

		/// <summary>
		/// Calls CreateSolarSystemPanel() which displays the PopUpPanel for change current solar system
		/// and with travelers. 
		/// </summary>
		/// <param name="sender">The action sender.</param>
		/// <param name="e">The action argument.</param>
		private void ShowSolarSystems(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			CreateSolarSystemPanel();
		}

		/// <summary>
		/// Calls CreateMissionPanel() which displays the PopUpPanel with the mission informations.
		/// </summary>
		/// <param name="sender">The action sender.</param>
		/// <param name="e">The action argument.</param>
		private void ShowMissionInfo(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			CreateMissionPanel();
		}
		#endregion


		#region Create Panel

		/// <summary>
		/// Creates PopUpPanel with the text as title. Checks if is the singleton panel and if it is not already open
		/// (if is throws ShutdownException). Calls GetUnusedPanelName to get name for this panel and register created panel
		/// to openPanelDict.
		/// </summary>
		/// <param name="text">The title text</param>
		/// <param name="type">The type of the creating panel.</param>
		/// <param name="isSingleton">Determines whether it is singlton panel.</param>
		/// <returns></returns>
		private Panel CreatePopUpPanel(string text, PanelType type, bool isSingleton) {
			Panel panel;
			if (isSingleton && openedPanelDict.ContainsKey(type.ToString())) {
				// The panel is already opened.
				throw new ShutdownException("Singleton PopUpPanel already exist.");
			} else {
				var name = GetUnusedPanelName(type.ToString(), isSingleton);
				panel = new PopUpPanel(screenWidth, screenHeight, text, name, textHeight + 1, skinDict["PanelR"], skinDict["Button"]);
				openedPanelDict.Add(name, panel);
			}
			return panel;
		}

		/// <summary>
		/// Creates a new singleton PopUpPanel with names of the actual saved mission and TextBox for insert the name of the save.
		/// </summary>
		private void CreateSavePanel() {
			// Checks if the panel is opened.
			if (openedPanelDict.ContainsKey(PanelType.SavePanel.ToString())) {
				return;
			}

			// Creates a PopUpPanel
			var panel = CreatePopUpPanel("Saves:", PanelType.SavePanel, true);
			gui.Controls.Add(panel);

			// Creates a new scrollable panel for names of the saved games.
			var scrollablePanel = CreateScrollablePanel(textHeight + 6, panel.Width - 28, panel.Height * 11 / 16);
			panel.Controls.Add(scrollablePanel);

			// Gets a file with saved games.
			var savePaths = Directory.GetFiles(Game.SavesGamePath, "*.save");

			// Inserts the names of the saves to the scrollable panel.
			for (int i = 0; i < savePaths.Length; i++) {
				var splited = savePaths[i].Split('\\');
				var label = CreateLabel(i * (textHeight + 1),
					scrollablePanel.Width, textHeight,
					splited[splited.Length - 1].Substring(0, splited[splited.Length - 1].Length - 5)
					);
				scrollablePanel.Controls.Add(label);
			}

			// Creates a textbox for entering the name of the save.
			var textArea = new TextBox() {
				Size = new Size(panel.Width - 28, panel.Height * 3 / 32),
				Location = new Point(10, panel.Height * 3 / 4),
				Skin = skinDict["PanelSkin"],
				TextStyle = new TextStyle {
					Alignment = Alignment.MiddleCenter
				},
				Padding = new Thickness(10, 0, 0, 0)
			};

			// Sets mouse actions to capture keyboard.
			textArea.MouseClick += CaptureKeyboard;
			textArea.MouseLeave += ReleaseKeyboard;

			panel.Controls.Add(textArea);

			// Creates a save button.
			var button = new SaveGameButton(PanelType.SavePanel.ToString(), textArea) {
				Location = new Point(panel.Width / 4, panel.Height * 7 / 8),
				Skin = skinDict["Button"],
				Text = "Ok",
				TextStyle = new TextStyle {
					Alignment = Alignment.MiddleCenter
				},
				Size = new Size(panel.Width / 3, panel.Height / 12)
			};
			panel.Controls.Add(button);
		}

		/// <summary>
		/// Creates a new singleton PopUpPanel to load a saved game or a new game (each type in own scrollable panel).
		/// </summary>
		private void CreateLoadPanel() {
			// Checks if the panel is opened.
			if (openedPanelDict.ContainsKey(PanelType.LoadPanel.ToString())) {
				return;
			}

			// Creates a PopUpPanel
			var panel = CreatePopUpPanel("Load saved mission:", PanelType.LoadPanel, true);
			gui.Controls.Add(panel);

			// Creates a new scrollable panel for names of the new games.
			var scrollablePanel = CreateScrollablePanel(textHeight + 6, panel.Width - 28, panel.Height / 3);
			panel.Controls.Add(scrollablePanel);

			// Gets a file with saved games.
			var missionPaths = Directory.GetFiles(Game.SavesGamePath, "*.save");

			// Inserts the names of the saves to the scrollable panel (SelectionLabels).
			for (int i = 0; i < missionPaths.Length; i++) {
				var splited = missionPaths[i].Split('\\');

				var label = CreateSelectionLabel(panel.Width, textHeight + 1, splited[splited.Length - 1].Substring(0, splited[splited.Length - 1].Length - 5),
					new Point(0, i * (textHeight + 1)), i, panel.Name, missionPaths[i]);
				label.TextStyle = new TextStyle {
					Alignment = Miyagi.Common.Alignment.TopCenter
				};
				scrollablePanel.Controls.Add(label);
			}

			var labelSave = CreateLabel(panel.Height / 2 - textHeight - 10, panel.Width, textHeight, "Load new mission:");
			panel.Controls.Add(labelSave);

			// Creates a new scrollable panel for a names of the saved games.
			scrollablePanel = CreateScrollablePanel(panel.Height / 2, panel.Width - 28, panel.Height / 3);
			panel.Controls.Add(scrollablePanel);

			// Gets a file with new games.
			missionPaths = Directory.GetFiles(Game.NewGamePath, "*.mission");

			// Inserts the names of the new games to the scrollable panel (SelectionLabels).
			for (int i = 0; i < missionPaths.Length; i++) {
				var splited = missionPaths[i].Split('\\');
				var label = CreateSelectionLabel(panel.Width, textHeight + 1, splited[splited.Length - 1].Substring(0, splited[splited.Length - 1].Length - 8),
					 new Point(0, i * (textHeight + 1)), i, panel.Name, missionPaths[i]);
				label.TextStyle = new TextStyle {
					Alignment = Miyagi.Common.Alignment.TopCenter
				};
				scrollablePanel.Controls.Add(label);
			}
		}

		/// <summary>
		/// Creates a new singleton PopUpPanel which shows current mission targets.
		/// </summary>
		private void CreateMissionPanel() {
			// Checks if the panel is opened.
			if (openedPanelDict.ContainsKey(PanelType.MissionInfoPanel.ToString())) {
				return;
			}

			// Creates a PopUpPanel
			var panel = CreatePopUpPanel("Mission Info:", PanelType.MissionInfoPanel, true);
			gui.Controls.Add(panel);

			// Creates a scrollable panel which shows current mission targets.
			var scrollablePanel = CreateScrollablePanel(textHeight + 6, panel.Width - 28, panel.Height * 37 / 48);
			panel.Controls.Add(scrollablePanel);

			// Inserts targets to the scrollable panel.
			var mission = Game.Mission;
			if (mission != null) {
				var targetList = mission.GetMissionInfo();
				int row = 0;
				foreach (var target in targetList) {
					var propLabel = CreatePropertyLabelGeneric<string>(0, row * (textHeight + 1), scrollablePanel.Width - 10, textHeight,
						target);
					scrollablePanel.Controls.Add(propLabel);
					row++;
				}
			}
		}


		/// <summary>
		/// Creates a new singleton PopUpPanel which shows solar systems and travelers.
		/// </summary>
		private void CreateSolarSystemPanel() {
			// Checks if the panel is opened.
			if (openedPanelDict.ContainsKey(PanelType.SolarSystemPanel.ToString())) {
				return;
			}

			// Creates a PopUpPanel
			var panel = CreatePopUpPanel(" Select solar system:", PanelType.SolarSystemPanel, true);
			gui.Controls.Add(panel);

			// Gets names of the solar systems.
			List<string> solarSystList = Game.SolarSystemManager.GetAllSolarSystemNames();

			int marginTop = textHeight + 6;

			// Creates a scrollable panel which shows solar system names (SelectionLabel - change current solar system)
			var scrollablePanel = CreateScrollablePanel(marginTop, panel.Width - 28, panel.Height / 2 - ((textHeight + 1) * 2));
			panel.Controls.Add(scrollablePanel);

			int selectionLabelMarginTop = textHeight + 1;
			int selectionLabelWidth = scrollablePanel.Width;

			// Inserts labels with names of the solar systems (SelectionLabels).
			for (int i = 0; i < solarSystList.Count; i++) {
				var selectionLabel = CreateSelectionLabel(
					selectionLabelWidth, textHeight, solarSystList[i],
					new Point(0, i * selectionLabelMarginTop), i, panel.Name
					);
				scrollablePanel.Controls.Add(selectionLabel);
			}

			marginTop = panel.Height / 2;

			// Title label
			var label = new Label() {
				Size = new Size(panel.Width / 2, textHeight + 1),
				Text = " Travelers:",
				Location = new Point(panel.Width / 4, marginTop),
				TextStyle = {
					Alignment = Miyagi.Common.Alignment.TopCenter
				}
			};
			panel.Controls.Add(label);

			marginTop += panel.Height / 10;

			// Creates a scrollable panel which shows travelers and their time to reach the destination (PropertyLabels).
			scrollablePanel = CreateScrollablePanel(marginTop, panel.Width - 28, panel.Height / 4);
			panel.Controls.Add(scrollablePanel);

			var travList = Game.SolarSystemManager.GetTravelers();
			var travelLabelWidth = selectionLabelWidth / 2;

			// Inserts labels with travelers (PropertyLabels).
			for (int i = 0; i < travList.Count; i++) {
				var textLabel = CreateLabel(i * selectionLabelMarginTop, travelLabelWidth, 25, travList[i].ToString());
				scrollablePanel.Controls.Add(textLabel);
				var travelerLabel = CreatePropertyLabelGeneric<TimeSpan>(scrollablePanel.Width / 2 + 10, i * selectionLabelMarginTop, travelLabelWidth, 25,
					travList[i].TimeProperty);
				scrollablePanel.Controls.Add(travelerLabel);
			}
		}

		/// <summary>
		/// Creates panel with scroll bars, setts padding (5,5,0,0) and border thickness (2).
		/// </summary>
		/// <param Name="marginTop">The top indent.</param>
		/// <param Name="width">The width of the panel.</param>
		/// <param Name="height">The height of the panel.</param>
		/// <returns>Returns Panel with both scroll bars.</returns>
		private Panel CreateScrollablePanel(int marginTop, int width, int height) {
			return new Panel() {
				Location = new Point(10, marginTop),
				Size = new Size(width, height),
				ResizeMode = ResizeModes.None,
				Skin = skinDict["PanelSkin"],
				BorderStyle = new BorderStyle { Thickness = new Thickness(2) },
				Padding = new Thickness(5, 5, 0, 0),
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
		#endregion


		#region Create Label

		/// <summary>
		/// Creates generic PropertyLabel with a runtime setted type.
		/// </summary>
		/// <param Name="marginLeft">The left indent.</param>
		/// <param Name="marginTop">The top indent.<param>
		/// <param Name="property">The showing property (Property).</param>
		/// <returns>The PropertyLabel with runtime setted type and reference to given property.</returns>
		private Label CreatePropertyLabel(int marginLeft, int marginTop, object property) {

			// Runtime choose type of the PropertyLabel depending on type of the Property.
			MethodInfo method = typeof(MyGUI).GetMethod("CreatePropertyLabelGeneric", BindingFlags.NonPublic | BindingFlags.Instance);
			var type = property.GetType().GetGenericArguments()[0];
			MethodInfo generic = method.MakeGenericMethod(type);

			// Creates arguments
			List<object> args = new List<object>();
			args.Add(marginLeft);
			args.Add(marginTop);
			args.Add(256);
			args.Add(26);
			args.Add(property);

			var o = generic.Invoke(this, args.ToArray());
			return (Label)o;
		}

		/// <summary>
		/// Creates label with middle-center alligment of the text.
		/// </summary>
		/// <param name="marginTop">The top indent.</param>
		/// <param name="width">The width of the label.</param>
		/// <param name="height">The height of the label.</param>
		/// <param name="text">The text to show.</param>
		/// <returns></returns>
		private Label CreateLabel(int marginTop, int width, int height, string text) {
			return CreateLabel(10, marginTop, width, height, text);
		}

		/// <summary>
		/// Creates label with middle-center alligment of the text.
		/// </summary>
		/// <param name="marginLeft">The left indent.</param>
		/// <param name="marginTop">The top indent.</param>
		/// <param name="width">The width of the label.</param>
		/// <param name="height">The height of the label.</param>
		/// <param name="text">The text to show.</param>
		/// <returns></returns>
		private Label CreateLabel(int marginLeft, int marginTop, int width, int height, string text) {
			var label = new Label() {
				Text = text,
				Size = new Size(width, height),
				Location = new Point(marginLeft, marginTop),
				TextStyle = {
					Alignment = Miyagi.Common.Alignment.MiddleCenter
				}
			};
			return label;
		}

		/// <summary>
		/// Creates SelectionLabel with sets MouseClick action by panelName (panelName represents PanelType).
		/// </summary>
		/// <param Name="width">The width of the Label.</param>
		/// <param Name="height">The height of the Label.</param>
		/// <param Name="text">The text in the Label.</param>
		/// <param Name="location">The relative position in the Panel.</param>
		/// <param Name="order">The number of the position.</param>
		/// <param name="panelName">The name of panel to close.</param>
		/// <param name="objectRef">The stored object.</param>
		/// <returns>Returns Label with setted MouseClick action</returns>
		private SelectionLabel CreateSelectionLabel(int width, int height, string text, Point location, int order, string panelName,
			object objectRef = null) {
			var selectLabel = new SelectionLabel(order, objectRef, panelName) {
				Size = new Size(width, height),
				Text = text,
				Location = location
			};
			return selectLabel;
		}

		/// <summary>
		/// Creates PropertyLabel without a title.
		/// </summary>
		/// <typeparam name="T">The type of Property (value)</typeparam>
		/// <param name="marginLeft">The left indent.</param>
		/// <param name="marginTop">The top indent.</param>
		/// <param name="width">The width of the label.</param>
		/// <param name="height">The height of the label.</param>
		/// <param name="property">The property which value will be shows like label text.</param>
		/// <returns>Returns PropertyLabel with setted property and without a title.</returns>
		private Label CreatePropertyLabelGeneric<T>(int marginLeft, int marginTop, int width, int height, Property<T> property) {
			var propLabel = new PropertyLabel<T>(property, "") {
				Size = new Size(width, height),
				Location = new Point(marginLeft, marginTop)
			};
			return propLabel;
		}
		#endregion


		#region Create Buttons

		/// <summary>
		/// Creates Button with middle-center alignment.
		/// </summary>
		/// <param Name="width">The width of the button.</param>
		/// <param Name="height">The height of the button.</param>
		/// <param Name="text">The button text.</param>
		/// <param Name="location">The relative position.</param>
		/// <returns>Returns Button with setted text and text alignment.</returns>
		private Button CreateButton(int width, int height, string text, Point location) {
			Button b = new Button() {
				Size = new Size(width, height),
				Location = location,
				Skin = skinDict["Button"],
				Text = text,
				TextStyle = {
					Alignment = Miyagi.Common.Alignment.MiddleCenter
				}
			};
			return b;
		}

		/// <summary>
		/// Creates Button with middle-center alignment which on MouseClick close panel (ClosePanel(name)).
		/// </summary>
		/// <param Name="width">The width of the button.</param>
		/// <param Name="height">The height of the button.</param>
		/// <param Name="text">The button text.</param>
		/// <param Name="location">The relative position.</param>
		/// <param name="panelName">The name of the closing panel.</param>
		/// <returns>Returns Button with setted text and on MouseClick action to close the panel.</returns>
		private CloseButton CreateCloseButton(int width, int height, string text, Point location, string panelName) {
			var b = new CloseButton(panelName) {
				Size = new Size(width, height),
				Location = location,
				Skin = skinDict["Button"],
				Text = text,
				TextStyle = new TextStyle {
					Alignment = Alignment.MiddleCenter
				},
				Name = panelName
			};
			return b;
		}

		/// <summary>
		/// Creates PictureBox and sets the action on MouseClick. Also loads the action icon to PictureBox.
		/// </summary>
		/// <param name="action">The action which is setted to PictureBox and called its OnMouseClick() on MouseClick.</param>
		/// <returns>Returns instance of the PictureBox that has setted the action.</returns>
		private PictureBox CreateGameActionIcon(IGameAction action) {
			var icon = new GameActionIconBox(action);
			return icon;
		}

		/// <summary>
		/// Creates Button and sets an action on MouseClick (QuitOnClick).
		/// </summary>
		/// <param Name="width">The width of the button.</param>
		/// <param Name="height">The height of the button.</param>
		/// <param Name="location">The relative position.</param>
		/// <returns>Returns Button with setted action and text Exit.</returns>
		private Button CreateExitButton(int width, int height, Point location) {
			var button = CreateButton(width, height, "Exit", location);
			button.MouseClick += QuitOnClick;
			return button;
		}
		#endregion

		/// <summary>
		/// Shows every given Property from the dictionary to propertyPanel. Each property on new line
		/// </summary>
		/// <param Name="propertiesDict">Dictionary with properties (key - Name, value - Property)</param>
		private void ShowGroupProperties(Dictionary<string, object> propertiesDict) {
			var propDict = propertiesDict;
			int marginLeft = propertyPanel.Width / 2;
			int marginTop = 26;
			int i = 0;

			// Add each property to propertyPanel (name and value in the one row)
			foreach (var property in propDict) {
				propertyPanel.Controls.Add(CreateLabel(marginTop * i, marginLeft, 26, property.Key));
				propertyPanel.Controls.Add(CreatePropertyLabel(marginLeft, marginTop * i, property.Value));
				++i;
			}
		}

		/// <summary>
		/// Creates an appropriate name a panel (singleton - just returns the name, else generates the unique name).
		/// </summary>
		/// <param name="name">The base of the name.</param>
		/// <param name="isSingleton">Determines whether it is singlton panel (singleton name).</param>
		/// <returns></returns>
		private string GetUnusedPanelName(string name, bool isSingleton) {
			if (isSingleton) {
				return name;
			}
			if (openedPanelDict.ContainsKey(name)) {
				return GetUnusedPanelName(name + 1, isSingleton);
			} else {
				return name;
			}
		}

		/// <summary>
		/// Disables all mission-contol buttons.
		/// </summary>
		private void DisableAllMissionControlButtons() {
			pauseButton.Enabled = false;
			saveButton.Enabled = false;
			solarSystemButton.Visible = false;
			missionButton.Visible = false;
		}
	}
}
