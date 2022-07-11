## modulify
Modulify framework that is a series of `IServiceProvider` in dotnet.

### Introduction
`modulify` consists of just simple `IModuleProvider`, `IModuleBuilder` and `IModuleCollection`.
and it's for implementing module-system to recognize and enumerate registered modules without module specific implementations.
Maybe, In common scenarios, these library is not needed. but, this can be an way to implement extensible editors or content managment systems.

### How to use
1. Registers the `Base-Type` to classify modules.
```
var Collection = new ModuleCollection();
if (Collection.BaseTypes.Add(typeof(MyModuleKind))) {
    // Here, a place that invoked when your module type registered in first-time.
    // TODO: Register initial modules.
}
```

2. Registers your own modules on the collection.
```
Collection.Add(new MyModule()); // --> MyModule inherits MyModuleKind.
```

3. Use your modules.
```
var Provider = ... /* an instance of IModuleProvider. */ ... ;
var Modules = Provider.FindAll(typeof(MyModuleKind));

Console.WriteLine("Supported `MyModuleKind` modules are:");
foreach(var Each in Modules.Select((X, i) => (Name: X.Name, Index: i)) {
    Console.WriteLine($"   {Each.Index + 1}. {Each.Name} supported.");
}
```

### Example: Document model.
In this scenario, the document needed to have `Serializer` module, `Document` module itself and `Document` model.
So, this example declares `IDocument`, `IDocumentModule` and, `IDocument{Text,Binary}Module` interfaces.

1. Write your document's class.
```
class MyDocument: IDocument {
    public Guid Guid { get; set; } // --> IDocument interface requires.
    public int Kind = 0;
    public string Author, Copyright;
    public string Content;
}
```

2. Registers your own `document` kind.
```
// --> and, the `document` kind itself,
abstract class MyDocumentKind : IDocumentBinaryModule {
   public virtual string Name => "My Document";
   public virtual string Alias => "my-document";
   public abstract bool CanSupport(IDocument Input);
   public abstract IDocument CreateNew();
  
   public bool CanSupports(Stream Input) {
      using var Reader = new BinaryReader(Stream, Encoding.UTF8, LeaveOpen: true);
      return CanSupports(Reader.Read7BitEncodedInt());
   }
   
   public void Serialize(Stream Input, IDocument Input) {
      if (Input is not MyDocument Document)
          throw new NotSupportedException();
   
      using (var Writer = new BinaryWriter(Stream, Encoding.UTF8, LeaveOpen: true))
      {
          Writer.Write7BitEncodedInt(Document.Kind);
          Writer.Write(Document.Author ?? string.Empty);
          Writer.Write(Document.Copyright ?? string.Empty);
          Writer.Write(Document.Content ?? string.Empty);
          
          Serialize(Writer, Document);
      }
   }
   
   public IDocument Deserialize(Stream Input) {
      using var Reader = new BinaryReader(Stream, Encoding.UTF8, LeaveOpen: true);
      if (!CanSupports(Reader.Read7BitEncodedInt())) {
          return null;
      }
      
      var Document = new MyDocument();
      Document.Author = Reader.ReadString();
      Document.Copyright = Reader.ReadString();
      Document.Content = Reader.ReadString();
      
      Deserialize(Reader, Document);
      return Document;
   }
   
   protected abstract bool CanSupports(int KindId);
   protected virtual void Serialize(BinaryWriter Writer, MyDocument Document) { }
   protected virtual void Deserialize(BinaryReader Reader, MyDocument Document) { }
}

// --> `default` document module.
class DefaultDocumentModule : MyDocumentKind {
   public override string Name => "Default (MyDocumentKind)";
   public override string Alias => "default@my-document";
   public override IDocument CreateNew() => new MyDocument();
   protected override bool CanSupports(int KindId) => KindId == 0;
}

// --> register written classes above.
Collection.BaseTypes.Add(typeof(MyDocumentKind));
Collection.AddDocument(new DefaultDocumentModule());
```

3. Use all document modules:
```
var Provider = ...;
var Kinds = Provider
  .FindAll(typeof(MyDocumentKind))
  .Select(X => X as MyDocumentKind);

foreach(var Each in Kinds) {
    Menu.Find("Create New...")
        .Add(Each.Name, () => {
            Workspace.OpenDocument(Each.CreateNew());
        });
        
   Menu.Find("Open Document")
       .Add(Each.Name, () => {
           var Ofd = new OpenFileDialog();
           // ..........
           
           using(var Stream = new FileStream(...)) {
               Workspace.OpenDocument(Each.Deserialize(Stream));
             }
       });
}
```

4. Extends your documents:
```
class ExtendedDocumentModule: MyDocumentKind {
   public override string Name => "Extended (MyDocumentKind)";
   public override string Alias => "extended@my-document";
   public override IDocument CreateNew() => new MyDocument();
   protected override bool CanSupports(int KindId) => KindId == 1;
   
   // TODO: overrides `Serialize`, `Deserialize` methods here...
}

Collection.AddDocument(new ExtendedDocumentModule());
```
