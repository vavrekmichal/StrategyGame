﻿using System;
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

namespace Strategy.GameGUI {

	public enum PanelType { MissionInfoPanel, LoadPanel, SavePanel, SolarSystemPanel, TravelPanel, MissionEndPanel }


	public class MyGUI : IGameGUI {

		Dictionary<PanelType, Panel> openedPanelDict;

		protected int screenWidth;
		protected int screenHeight;

		protected FlowLayoutPanel upperMenu;
		protected FlowLayoutPanel mainMenu;
		protected Label nameOfSolarSystem;

		private Panel buttonsPanel;
		private Panel statPanel;
		private Panel actionPanel;

		private bool enable;
		private Button pauseButton;
		private Button loadButton;
		private Button saveButton;
		private Button missionButton;
		private Button travelButton;

		private Label statPanelName;
		private Panel propertyPanel;

		private Panel gameActionPanel;

		private Panel materialPanel;

		private Panel console;
		private int consoleLinesNumber;

		Dictionary<string, Skin> skinDict;
		Dictionary<string, Font> fonts;
		MiyagiSystem guiSystem;
		GUI gui;

		private const string nothingSelected = "	Nothing selected";
		private const int textHeight = 18;


		/// <summary>
		/// Constructor initializes GUI systrem for Mogre and Load fonts, skins and create control panel.
		/// Also the constuctor creates mouse bounds (app using Miagi cursor for control)
		/// </summary>
		/// <param Name="width">Window width</param>
		/// <param Name="height">Window height</param>
		/// <param Name="mouse">Mogre mouse input</param>
		/// <param Name="keyboard">Mogre keyboard input</param>
		/// <param Name="groupMgr">GroupManager instance</param>
		public MyGUI(int width, int height, MOIS.Mouse mouse, MOIS.Keyboard keyboard) {
			screenHeight = height;
			screenWidth = width;

			openedPanelDict = new Dictionary<PanelType, Panel>();

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
			Cursor cursor = new Cursor(cursorSkin, new Size(30, 30), new Point(0, 0), true);

			guiSystem.GUIManager.Cursor = cursor;   // Set cursor

			CreateMainMenu();
			CreateTopMenu();

			CreateCameraBounds();				// Create bounds around screen to move the camera
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
			topSection.MouseHover += MouseOver;
			topSection.MouseLeave += MouseLeave;
			gui.Controls.Add(topSection);

			// Botton bound
			var backSection = new Button() {
				Size = new Size(screenWidth * 13 / 15, screenHeight / 10),
				Location = new Point(screenWidth / 15, screenHeight * 7 / 10),
				Name = "backSection"
			};
			backSection.MouseHover += MouseOver;
			backSection.MouseLeave += MouseLeave;
			gui.Controls.Add(backSection);

			// Left bound
			var leftSection = new Button() {
				Size = new Size(screenWidth / 15, screenHeight - (screenHeight * 4 / 10)),
				Location = new Point(0, screenHeight / 10),
				Name = "leftSection"
			};
			leftSection.MouseHover += MouseOver;
			leftSection.MouseLeave += MouseLeave;
			gui.Controls.Add(leftSection);

			// Right bound
			var rightSection = new Button() {
				Size = new Size(screenWidth / 15, screenHeight - (screenHeight * 4 / 10)),
				Location = new Point(screenWidth * 14 / 15, screenHeight / 10),
				Name = "rightSection"
			};
			rightSection.MouseHover += MouseOver;
			rightSection.MouseLeave += MouseLeave;
			gui.Controls.Add(rightSection);

			// Rigth-up corner bound
			var rightUpSection = new Button() {
				Size = new Size(screenWidth / 15, screenHeight / 10),
				Location = new Point(screenWidth * 14 / 15, 0),
				Name = "rightUpSection"
			};
			rightUpSection.MouseHover += MouseOver;
			rightUpSection.MouseLeave += MouseLeave;
			gui.Controls.Add(rightUpSection);

			// Right-down corner bound
			var rightDownSection = new Button() {
				Size = new Size(screenWidth / 15, screenHeight / 10),
				Location = new Point(screenWidth * 14 / 15, screenHeight * 7 / 10),
				Name = "rightDownSection"
			};
			rightDownSection.MouseHover += MouseOver;
			rightDownSection.MouseLeave += MouseLeave;
			gui.Controls.Add(rightDownSection);

			// Left-down corner bound
			var leftDownSection = new Button() {
				Size = new Size(screenWidth / 15, screenHeight / 10),
				Location = new Point(0, screenHeight * 7 / 10),
				Name = "leftDownSection"
			};
			leftDownSection.MouseHover += MouseOver;
			leftDownSection.MouseLeave += MouseLeave;
			gui.Controls.Add(leftDownSection);

			// Left-up corner bound
			var leftUpSection = new Button() {
				Size = new Size(screenWidth / 15, screenHeight / 10),
				Location = new Point(0, 0),
				Name = "leftUpSection"
			};
			leftUpSection.MouseHover += MouseOver;
			leftUpSection.MouseLeave += MouseLeave;
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

			if (group.OwnerTeam.Name == Game.PlayerName) {
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
			if (group.OwnerTeam.Name == Game.PlayerName) {
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
			// CreatePropertyLabelAsLabel is private function

			MethodInfo method = typeof(MyGUI).GetMethod("CreatePropertyLabelAsLabel", BindingFlags.NonPublic | BindingFlags.Instance);
			var type = property.GetType().GetGenericArguments()[0];
			MethodInfo generic = method.MakeGenericMethod(type);
			List<object> args = new List<object>();
			args.Add(marginLeft);
			args.Add(marginTop);
			args.Add(256);
			args.Add(26);
			args.Add(property);
			var o = generic.Invoke(this, args.ToArray());
			return o;
		}

		#region top panel
		/// <summary>
		/// Creates top bar
		/// </summary>
		private void CreateTopMenu() {
			// Create top panel
			upperMenu = new FlowLayoutPanel() {
				Size = new Size(screenWidth * textHeight / 20, screenHeight / 15),
				Location = new Point(screenWidth / 20, 0),
				Skin = skinDict["Panel"],
				Opacity = 0.5f
			};
			gui.Controls.Add(upperMenu);

			// Left Label with text "Current solar system:"
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

			// Label to show actual solar system Name
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
					Alignment = Miyagi.Common.Alignment.MiddleLeft,
					ForegroundColour = Colours.White
				},
				Padding = new Thickness(1)
			};
			upperMenu.Controls.Add(materialIntro);

			// Create materialBox - ScrollablePanel
			materialPanel = new Panel() {
				Size = new Size(upperMenu.Width * 2 / 5, upperMenu.Height),
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
			upperMenu.Controls.Add(materialPanel);
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
			int buttonMarginLeft = buttonsPanel.Width / 9;
			int buttonMarginLeftSecond = buttonsPanel.Width / 2 + buttonMarginLeft / 2;
			int buttonMarginTop = buttonsPanel.Height * 6 / 25;
			int row = 0;

			int buttonWidth = buttonsPanel.Width / 3;

			pauseButton = CreateButton(buttonWidth, buttonsPanel.Height / 5, "Pause BUTTON", new Point(buttonMarginLeft, buttonMarginTop * row));
			buttonsPanel.Controls.Add(pauseButton);
			pauseButton.MouseClick += Pause;
			pauseButton.Enabled = false;
			row++;

			saveButton = CreateButton(buttonWidth, buttonsPanel.Height / 5, "Save", new Point(buttonMarginLeft, buttonMarginTop * row));
			buttonsPanel.Controls.Add(saveButton);
			saveButton.MouseClick += ShowSavePanel;
			saveButton.Visible = false;

			loadButton = CreateButton(buttonWidth, buttonsPanel.Height / 5, "Load", new Point(buttonMarginLeft, buttonMarginTop * row));
			buttonsPanel.Controls.Add(loadButton);
			loadButton.MouseClick += ShowLoadPanel;
			row++;

			Button b = CreateButton(buttonWidth, buttonsPanel.Height / 5, "End Mission", new Point(buttonMarginLeft, buttonMarginTop * row));
			buttonsPanel.Controls.Add(b);
			b.MouseClick += EndMission;
			row++;

			b = CreateExitButton(buttonWidth, buttonsPanel.Height / 5, new Point(buttonMarginLeft, buttonMarginTop * row));
			buttonsPanel.Controls.Add(b);

			row = 0;
			missionButton = CreateButton(buttonWidth, buttonsPanel.Height / 5, "Mission info", new Point(buttonMarginLeftSecond, buttonMarginTop * row));
			buttonsPanel.Controls.Add(missionButton);
			missionButton.MouseClick += ShowMissionInfo;
			missionButton.Visible = false;
			row++;

			travelButton = CreateButton(buttonWidth, buttonsPanel.Height / 5, "Solar systems", new Point(buttonMarginLeftSecond, buttonMarginTop * row));
			buttonsPanel.Controls.Add(travelButton);
			travelButton.MouseClick += ShowSolarSystems;
			travelButton.Visible = false;
			row++;



			return buttonsPanel;
		}

		private Button CreateExitButton(int width, int height, Point location) {
			var button = CreateButton(width, height, "Exit", location);
			button.MouseClick += QuitOnClick;
			return button;
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


		private void ShowSavePanel(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			CreateSavePanel();
		}

		private void ShowLoadPanel(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			CreateLoadPanel();
		}

		private void EndMission(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			foreach (var item in new Dictionary<PanelType, Panel>(openedPanelDict)) {
				item.Value.Dispose();
				openedPanelDict.Remove(item.Key);
			}
			Game.DestroyGame();
		}


		//private void SelectLoadMission(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
		//	// Button Action for SelectionLabel calls Game.Load and load given mission
		//	var selectionLabel = sender as SelectionLabel;
		//	if (selectionLabel != null) {
		//		selectionLabel.ClosePanel();
		//		Game.Load((string)selectionLabel.StoredObject);
		//	}
		//}

		//private void SelectSolarSystem(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
		//	// Button Action for SelectionLabel calls GroupManager function ChangeSolarSystem and close Panel
		//	var selectionLabel = sender as SelectionLabel;
		//	if (selectionLabel != null) {
		//		Game.GroupManager.ChangeSolarSystem(selectionLabel.NumberOfItem);
		//		selectionLabel.ClosePanel();
		//	}
		//}

		//private void Travel(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
		//	// Function calls CreateTraveler with selected number of SolarSystem and traveler(IMovableGameObject)
		//	var selectionLabel = sender as SelectionLabel;
		//	if (selectionLabel != null) {
		//		Game.GroupManager.CreateTraveler(selectionLabel.NumberOfItem, selectionLabel.StoredObject);
		//		selectionLabel.ClosePanel();
		//	}
		//}

		private void QuitOnClick(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			// Function closes application (throw ShutdownException)
			guiSystem.Dispose();
			throw new Strategy.Exceptions.ShutdownException();
		}

		private void CaptureKeyboard(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			var casted = sender as TextBox;
			if (casted != null) {
				casted.Focused = true;
			}
			Game.KeyboardCaptured = true;
		}

		private void ReleaseKeyboard(object sender, Miyagi.Common.Events.MouseEventArgs e) {
			var casted = sender as TextBox;
			if (casted != null) {
				casted.Focused = false;
			}
			Game.KeyboardCaptured = false;
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

		private void ShowSolarSystems(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			// Shows Panel with SolarSystems and travelers
			CreateSolarSystemPanel();
		}
		private void ShowMissionInfo(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			// Shows Panel with Mission info
			CreateMissionPanel();
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

		//private PopUpPanelControl savePanelIsClosed = new PopUpPanelControl(true);

		private void CreateSavePanel() {
			if (openedPanelDict.ContainsKey(PanelType.SavePanel)) {
				return;
			}
			var panel = CreatePopUpPanel("Saves:", PanelType.SavePanel);

			var innerScrollablePanel = CreateInnerScrollablePanel(textHeight + 6, panel.Width - 28, panel.Height * 11 / 16);
			var savePaths = Directory.GetFiles(Game.SavesGamePath, "*.save");

			for (int i = 0; i < savePaths.Length; i++) {
				var splited = savePaths[i].Split('\\');
				var label = CreateLabel(i * (textHeight + 1),
					innerScrollablePanel.Width, textHeight,
					splited[splited.Length - 1].Substring(0, splited[splited.Length - 1].Length - 5)
					);
				innerScrollablePanel.Controls.Add(label);
			}
			panel.Controls.Add(innerScrollablePanel);

			var textArea = new TextBox() {
				Size = new Size(panel.Width - 28, panel.Height * 3 / 32),
				Location = new Point(10, panel.Height * 3 / 4),
				Skin = skinDict["PanelSkin"],
				TextStyle = new TextStyle {
					Alignment = Alignment.MiddleCenter
				},
				Padding = new Thickness(10, 0, 0, 0)
			};

			textArea.MouseClick += CaptureKeyboard;
			textArea.MouseLeave += ReleaseKeyboard;

			panel.Controls.Add(textArea);

			var button = new SaveGameButton(PanelType.SavePanel, textArea) {
				Location = new Point(panel.Width / 4, panel.Height * 7 / 8),
				Skin = skinDict["Button"],
				Text = "Ok",
				TextStyle = new TextStyle {
					Alignment = Alignment.MiddleCenter
				},
				Size = new Size(panel.Width / 3, panel.Height / 12)
			};

			panel.Controls.Add(button);
			openedPanelDict.Add(PanelType.SavePanel, panel);
			gui.Controls.Add(panel);
		}


		//private PopUpPanelControl loadPanelIsClosed = new PopUpPanelControl(true);

		private void CreateLoadPanel() {
			if (openedPanelDict.ContainsKey(PanelType.LoadPanel)) {
				return;
			}

			var panel = CreatePopUpPanel("Load new mission:", PanelType.LoadPanel);

			var innerScrollablePanel = CreateInnerScrollablePanel(textHeight + 6, panel.Width - 28, panel.Height / 3);
			var missionPaths = Directory.GetFiles(Game.SavesGamePath, "*.save");

			for (int i = 0; i < missionPaths.Length; i++) {
				var splited = missionPaths[i].Split('\\');

				var label = CreateSelectionLabel(panel.Width, textHeight + 1, splited[splited.Length - 1].Substring(0, splited[splited.Length - 1].Length - 5),
					new Point(0, i * (textHeight + 1)), i, PanelType.LoadPanel, missionPaths[i]);
				label.TextStyle = new TextStyle {
					Alignment = Miyagi.Common.Alignment.TopCenter
				};
				innerScrollablePanel.Controls.Add(label);
			}
			panel.Controls.Add(innerScrollablePanel);

			var labelSave = CreateLabel(panel.Height / 2 - textHeight - 10, panel.Width, textHeight, "Load saved mission:");

			panel.Controls.Add(labelSave);

			innerScrollablePanel = CreateInnerScrollablePanel(panel.Height / 2, panel.Width - 28, panel.Height / 3);
			missionPaths = Directory.GetFiles(Game.NewGamePath, "*.mission");

			for (int i = 0; i < missionPaths.Length; i++) {
				var splited = missionPaths[i].Split('\\');

				var label = CreateSelectionLabel(panel.Width, textHeight + 1, splited[splited.Length - 1].Substring(0, splited[splited.Length - 1].Length - 8),
					 new Point(0, i * (textHeight + 1)), i, PanelType.LoadPanel, missionPaths[i]);
				label.TextStyle = new TextStyle {
					Alignment = Miyagi.Common.Alignment.TopCenter
				};
				innerScrollablePanel.Controls.Add(label);
			}
			panel.Controls.Add(innerScrollablePanel);
			openedPanelDict.Add(PanelType.LoadPanel, panel);
			gui.Controls.Add(panel);
		}



		private void CreateMissionPanel() {
			if (openedPanelDict.ContainsKey(PanelType.MissionInfoPanel)) {
				return;
			}

			var panel = CreatePopUpPanel("Mission Info:", PanelType.MissionInfoPanel);

			var innerScrollablePanel = CreateInnerScrollablePanel(textHeight + 6, panel.Width - 28, panel.Height * 37 / 48);


			panel.Controls.Add(innerScrollablePanel);
			var mission = Game.Mission;
			if (mission != null) {
				var targetList = mission.GetMissionInfo();

				int row = 0;
				foreach (var target in targetList) {
					innerScrollablePanel.Controls.Add(CreatePropertyLabel<string>(innerScrollablePanel.Width - 10, textHeight, "Target " + row + ": ",
						new Point(0, row * (textHeight + 1)), target));
					row++;
				}
			}
			openedPanelDict.Add(PanelType.MissionInfoPanel, panel);
			gui.Controls.Add(panel);
		}


		//private PopUpPanelControl solarSystPanelIsClosed = new PopUpPanelControl(true);

		/// <summary>
		/// Creates top bar with info about current SolarSystem and players materials
		/// </summary>
		private void CreateSolarSystemPanel() {
			if (openedPanelDict.ContainsKey(PanelType.SolarSystemPanel)) {
				return;
			}
			var panel = CreatePopUpPanel(" Select solar system:", PanelType.SolarSystemPanel);

			List<string> solarSystList = Game.GroupManager.GetAllSolarSystemNames();

			int marginTop = 5 + textHeight + 1;
			var innerScrollablePanel = CreateInnerScrollablePanel(marginTop, panel.Width - 28, panel.Height / 2 - ((textHeight + 1) * 2));

			panel.Controls.Add(innerScrollablePanel);

			innerScrollablePanel.Padding = new Thickness(10, 5, 0, 0);

			int selectionLabelMarginTop = textHeight + 1;
			int selectionLabelWidth = innerScrollablePanel.Width;

			// Labels with Name of solar systems
			for (int i = 0; i < solarSystList.Count; i++) {
				var selectionLabel = CreateSelectionLabel(
					selectionLabelWidth, textHeight, solarSystList[i],
					new Point(0, i * selectionLabelMarginTop), i, PanelType.SolarSystemPanel
					);

				innerScrollablePanel.Controls.Add(selectionLabel);
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
			openedPanelDict.Add(PanelType.SolarSystemPanel, panel);
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
		private SelectionLabel CreateSelectionLabel(int width, int height, string text, Point location, int order, PanelType panelToClose,
			object objectRef = null) {
			var selectLabel = new SelectionLabel(order, objectRef, panelToClose) {
				Size = new Size(width, height),
				Text = text,
				Location = location

			};
			return selectLabel;
		}

		///// <summary>
		///// Create SelectionLabel - on click Select positon and can close setted panel
		///// </summary>
		///// <param Name="width">Width of Label</param>
		///// <param Name="height">Height of Label</param>
		///// <param Name="text">Text in Label</param>
		///// <param Name="location">Relative position in Panel</param>
		///// <param Name="order">Number of choice</param>
		///// <param Name="panelToClose">Closing Panel</param>
		///// <returns></returns>
		//private SelectionLabel CreateSelectionLabel(int width, int height, string text, Point location, int order, Panel panelToClose, PopUpPanelControl controler,
		//	object objectRef = null) {
		//	var selectLabel = new SelectionLabel(order, objectRef, panelToClose, controler) {
		//		Size = new Size(width, height),
		//		Text = text,
		//		Location = location

		//	};
		//	return selectLabel;
		//}

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
				Text = text,
				TextStyle = {
					Alignment = Miyagi.Common.Alignment.MiddleCenter
				}
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
				Location = new Point(marginLeft, marginTop),
				TextStyle = {
					Alignment = Miyagi.Common.Alignment.MiddleCenter
				}
			};
			return label;
		}

		private Panel CreatePopUpPanel(string text, PanelType type) {

			var panel = new Panel() {
				Width = screenWidth / 2,
				Height = screenHeight * 4 / 7,
				Location = new Point(screenWidth / 4, screenHeight / 5),
				Skin = skinDict["PanelR"],
				ResizeMode = ResizeModes.None,
				Padding = new Thickness(5, 10, 0, 0)
			};

			// Title label
			var label = new Label() {
				Size = new Size(panel.Width / 2, textHeight + 1),
				Text = text,
				Location = new Point(panel.Width / 4, 0),
				TextStyle = {
					Alignment = Miyagi.Common.Alignment.TopCenter
				}
			};

			panel.Controls.Add(label);

			var closeButton = CreateCloseButton(panel.Width / 3,
				panel.Height / 12,
				"Cancel",
				new Point(panel.Width * 5 / 8, panel.Height * 7 / 8),
					type);
			panel.Controls.Add(closeButton);

			return panel;
		}

		private CloseButton CreateCloseButton(int width, int height, string text, Point location, PanelType panelToClose) {
			var b = new CloseButton(panelToClose) {
				Size = new Size(width, height),
				Location = location,
				Skin = skinDict["Button"],
				Text = text,
				TextStyle = new TextStyle {
					Alignment = Alignment.MiddleCenter
				}
			};
			return b;
		}

		public int NumberOfPopUpPanels {
			get { return openedPanelDict.Count; }
		}

		/// <summary>
		/// Dispose GUI system
		/// </summary>
		public void Dispose() {
			guiSystem.Dispose();
		}

		/// <summary>
		/// Function updete GUI
		/// </summary>
		public void Update() {
			guiSystem.Update();
		}


		public void ShowTravelSelectionPanel(List<string> possibilities, string topic, object gameObject) {

			var panel = CreatePopUpPanel(" " + topic, PanelType.TravelPanel); // Don't need know if panel is closed or not
			gui.Controls.Add(panel);

			int marginTop = 10;

			marginTop += textHeight + 1;

			var innerScrollablePanel = CreateInnerScrollablePanel(marginTop, panel.Width - 28, panel.Height * 37 / 48);

			panel.Controls.Add(innerScrollablePanel);

			innerScrollablePanel.Padding = new Thickness(10, 5, 0, 0);

			int selectionLabelMarginTop = 26;
			int selectionLabelWidth = innerScrollablePanel.Width;

			// Create SelectionLabels with names of SolarSystems
			for (int i = 0; i < possibilities.Count; i++) {

				SelectionLabel selectionLabel = CreateSelectionLabel(
					selectionLabelWidth, 25, possibilities[i],
					new Point(0, i * selectionLabelMarginTop), i, PanelType.TravelPanel, gameObject
					);

				innerScrollablePanel.Controls.Add(selectionLabel);
			}



		}

		/// <summary>
		/// Prints given text to panel in GUI.
		/// </summary>
		/// <param name="text">Text to be printed.</param>
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
		/// <param name="actionList">List that contains the IGameAction to display.</param>
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

		/// <summary>
		/// Reloads materialPanel (adds names and values for each given material).
		/// </summary>
		/// <param name="materialDict">Dictionary that contains the IMaterials to display.</param>
		public void UpdatePlayerMaterialDict(Dictionary<string, IMaterial> materialDict) {
			int row = 0;

			foreach (var materialPair in materialDict) {
				var nameLabel = CreateLabel(row * (textHeight + 1), materialPanel.Width / 3, textHeight, materialPair.Key);
				var valueLabel = CreatePropertyLabelAsLabel<int>(materialPanel.Width / 2, row * (textHeight + 1),
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

		public bool Enable {
			get { return enable; }
			set {
				pauseButton.Enabled = value;
				loadButton.Visible = !value;
				saveButton.Visible = value;
				travelButton.Visible = value;
				missionButton.Visible = value;
				enable = value;
			}
		}

		public void End(string printText) {

			if (openedPanelDict.ContainsKey(PanelType.MissionEndPanel)) {
				return;
			}

			Game.Pause(true);

			ClearMissionData();
			DisableAll();

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
			var button = CreateCloseButton(panel.Width / 4, panel.Height / 6, "Ok", new Point(panel.Width * 3 / 8, panel.Height * 3 / 4), PanelType.MissionEndPanel);
			button.MouseClick += EndMission;
			panel.Controls.Add(button);
			openedPanelDict.Add(PanelType.MissionEndPanel, panel);
			gui.Controls.Add(panel);

		}

		private void DisableAll() {
			pauseButton.Enabled = false;
			saveButton.Enabled = false;
			travelButton.Visible = false;
			missionButton.Visible = false;
		}

		public void ClearMissionData() {
			materialPanel.Controls.Clear();
			console.Controls.Clear();
			consoleLinesNumber = 0;
		}

		public void ClosePanel(PanelType type) {
			if (openedPanelDict.ContainsKey(type)) {
				openedPanelDict[type].Dispose();
				openedPanelDict.Remove(type);
			}
		}
	}
}
