using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;
using Strategy.Exceptions;
using Strategy.GameObjectControl.Game_Objects.GameActions;
using Strategy.GameObjectControl.Game_Objects.GameSave;
using Strategy.GameObjectControl.Game_Objects.GameTargets;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.TeamControl;

namespace Strategy.GameObjectControl.Game_Objects.GameLoad {
	/// <summary>
	/// Compiles the classes in runtime and creating the instance of them.
	/// </summary>
	public class RunTimeCreator {

		private Dictionary<string, int> usedNameDict;

		private AssemblyBuilder assemblyBuilder;
		private ModuleBuilder moduleBuilder;
		private List<MetadataReference> metadataRef;
		private CompilationOptions comilationOption;
		private List<string> isCompiled;

		/// <summary>
		/// Initializes the runtime compiler and metadata references for dynamic assembly.
		/// </summary>
		public RunTimeCreator() {
			usedNameDict = new Dictionary<string, int>();
			// Set references to runtime compiling
			isCompiled = new List<string>();
			var t = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
			metadataRef = new List<MetadataReference>();
			metadataRef.Add(MetadataFileReference.CreateAssemblyReference("mscorlib"));
			metadataRef.Add(new MetadataFileReference(typeof(Strategy.MyMogre).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(Strategy.Game).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(GameObjectControl.Game_Objects.StaticGameObjectBox.StaticGameObject).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(Team).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(System.Linq.Enumerable).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(LinkedList<>).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(Path.GetFullPath((new Uri(t + "\\\\Mogre.dll")).LocalPath)));
			metadataRef.Add(new MetadataFileReference(typeof(PropertyManager).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(GameObjectControl.Game_Objects.StaticGameObjectBox.IStaticGameObject).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(Strategy.Game).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(ActionAnswer).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(PropertyEnum).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(XmlLoadException).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(ConstructorFieldAttribute).Assembly.Location));

			comilationOption = new CompilationOptions(OutputKind.DynamicallyLinkedLibrary);

			assemblyBuilder =
				AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("DynamicAssembly" + Guid.NewGuid()),
															  AssemblyBuilderAccess.RunAndCollect);
			moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");
		}

		#region Public
		/// <summary>
		/// Compiles all objects which are used in a mission.
		/// </summary>
		/// <param name="syntaxTreeList">The List with SyntaxTrees (compiling classes).</param>
		public void CompileUsedObjects(List<SyntaxTree> syntaxTreeList) {
			var comp = Compilation.Create("Test.dll"
							 , syntaxTrees: syntaxTreeList
							 , references: metadataRef
							 , options: comilationOption
							 );

			// Runtime compilation and check errors
			var result = comp.Emit(moduleBuilder);

			if (!result.Success) {
				foreach (var d in result.Diagnostics) {
					Console.WriteLine(d);
				}
				throw new XmlLoadException("Compilation failed ");
			}
		}

		/// <summary>
		/// Creates IStaticGameObject by the given type name.
		/// </summary>
		/// <param name="fullName">The name og the creating type.</param>
		/// <param name="args">The arguments of creating object.</param>
		/// <returns>Returns created IStaticGameObject.</returns>
		public IStaticGameObject CreateISGO(string fullName, object[] args) {
			IStaticGameObject isgo = (IStaticGameObject)CreateObject(fullName, args);
			return isgo;
		}

		/// <summary>
		/// Creates IMovableGameObject by the given type name.
		/// </summary>
		/// <param name="fullName">The name og the creating type.</param>
		/// <param name="args">The arguments of creating object.</param>
		/// <returns>Returns created IMovableGameObject.</returns>
		public IMovableGameObject CreateIMGO(string fullName, object[] args) {
			IMovableGameObject imgo = (IMovableGameObject)CreateObject(fullName, args);
			return imgo;
		}

		/// <summary>
		/// Creates IGameAction by the given type name.
		/// </summary>
		/// <param name="fullName">The name og the creating type.</param>
		/// <param name="args">The arguments of creating object.</param>
		/// <returns>Returns created IGameAction.</returns>
		public IGameAction CreateIGameAction(string fullName, object[] args) {
			return (IGameAction)CreateObject(fullName, args);
		}

		/// <summary>
		/// Creates ITarget by the given type name.
		/// </summary>
		/// <param name="fullName">The name og the creating type.</param>
		/// <param name="args">The arguments of creating object.</param>
		/// <returns>Returns created ITarget.</returns>
		public ITarget CreateITarget(string fullName, object[] args) {
			return (ITarget)CreateObject(fullName, args);
		}

		/// <summary>
		/// Creates special type Gate (is not runtime compiled). 
		/// </summary>
		/// <param Name="solarSystName">The name of SolarSystem where the Gate will be.</param>
		/// <returns>Returns instance of Gate</returns>
		public Gate CreateGate(string solarSystName, Team teamNone) {
			var gate = new Gate("Gate " + solarSystName,
				teamNone);
			gate.Team.AddISGO(gate);
			return gate;
		}

		/// <summary>
		/// Returns the unused name with the given base.
		/// </summary>
		/// <param name="name">The base of the name.</param>
		/// <returns>Returns the unused name with the given base.</returns>
		public string GetUnusedName(string name) {
			if (usedNameDict.ContainsKey(name)) {
				usedNameDict[name]++;
				if (usedNameDict.ContainsKey(name + usedNameDict[name])) {
					return GetUnusedName(name + usedNameDict[name]);
				}
				return name + usedNameDict[name];
			} else {
				usedNameDict.Add(name, 0);
				return name;
			}
		}

		#endregion

		/// <summary>
		/// Creates a object in runtime from dynamic 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="fullPath"></param>
		/// <param name="fullName"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		private object CreateObject(string fullName, object[] args) {

			var o = moduleBuilder.GetType(fullName);

			object runTimeObject;
			runTimeObject = Activator.CreateInstance(o, args);
			return runTimeObject;
		}
	}
}
