// Smdn.Text.Ondulish.dll (Smdn.Text.Ondulish-4.0.0)
//   Name: Smdn.Text.Ondulish
//   AssemblyVersion: 4.0.0.0
//   InformationalVersion: 4.0.0+83e998fd2f6e3822969df4b1b14bf2fab381d0f5
//   TargetFramework: .NETStandard,Version=v2.1
//   Configuration: Release
#nullable enable annotations

using System;
using System.Collections.Generic;
using System.IO;
using MeCab;

namespace MeCab {
  public class DictionaryInfo : IDisposable {
    protected bool swigCMemOwn;

    public DictionaryInfo() {}

    public string charset { get; }
    public string filename { get; }
    public uint lsize { get; }
    public DictionaryInfo next { get; }
    public uint rsize { get; }
    public uint size { get; }
    public int type { get; }
    public ushort version { get; }

    protected virtual void Dispose(bool disposing) {}
    public void Dispose() {}
    ~DictionaryInfo() {}
  }

  public class Lattice : IDisposable {
    protected bool swigCMemOwn;

    public Lattice() {}

    protected virtual void Dispose(bool disposing) {}
    public void Dispose() {}
    ~Lattice() {}
    public virtual double Z() {}
    public virtual void add_request_type(int request_type) {}
    public virtual Node begin_nodes(uint pos) {}
    public virtual Node bos_node() {}
    public virtual int boundary_constraint(uint pos) {}
    public virtual void clear() {}
    public virtual Node end_nodes(uint pos) {}
    public virtual string enumNBestAsString(uint N) {}
    public virtual Node eos_node() {}
    public virtual string feature_constraint(uint pos) {}
    public virtual bool has_constraint() {}
    public virtual bool has_request_type(int request_type) {}
    public virtual bool is_available() {}
    public virtual Node newNode() {}
    public virtual bool next() {}
    public virtual void remove_request_type(int request_type) {}
    public virtual int request_type() {}
    public virtual string sentence() {}
    public virtual void set_Z(double Z) {}
    public virtual void set_boundary_constraint(uint pos, int boundary_constraint_type) {}
    public virtual void set_feature_constraint(uint begin_pos, uint end_pos, string feature) {}
    public virtual void set_request_type(int request_type) {}
    public virtual void set_result(string result) {}
    public void set_sentence(string sentence) {}
    public virtual void set_theta(float theta) {}
    public virtual void set_what(string str) {}
    public virtual uint size() {}
    public virtual float theta() {}
    public virtual string toString() {}
    public virtual string toString(Node node) {}
    public virtual string what() {}
  }

  public class MeCab {
    public static readonly int MECAB_ALLOCATE_SENTENCE = 64;
    public static readonly int MECAB_ALL_MORPHS = 32;
    public static readonly int MECAB_ALTERNATIVE = 16;
    public static readonly int MECAB_ANY_BOUNDARY = 0;
    public static readonly int MECAB_BOS_NODE = 2;
    public static readonly int MECAB_EON_NODE = 4;
    public static readonly int MECAB_EOS_NODE = 3;
    public static readonly int MECAB_INSIDE_TOKEN = 2;
    public static readonly int MECAB_MARGINAL_PROB = 8;
    public static readonly int MECAB_NBEST = 2;
    public static readonly int MECAB_NOR_NODE = 0;
    public static readonly int MECAB_ONE_BEST = 1;
    public static readonly int MECAB_PARTIAL = 4;
    public static readonly int MECAB_SYS_DIC = 0;
    public static readonly int MECAB_TOKEN_BOUNDARY = 1;
    public static readonly int MECAB_UNK_DIC = 2;
    public static readonly int MECAB_UNK_NODE = 1;
    public static readonly int MECAB_USR_DIC = 1;
    public static readonly string VERSION = "0.996";

    public MeCab() {}
  }

  public class Model : IDisposable {
    public static string version() {}

    protected bool swigCMemOwn;

    public Model() {}
    public Model(string argc) {}

    protected virtual void Dispose(bool disposing) {}
    public void Dispose() {}
    ~Model() {}
    public virtual Lattice createLattice() {}
    public virtual Tagger createTagger() {}
    public virtual DictionaryInfo dictionary_info() {}
    public virtual Node lookup(string begin, string end, Lattice lattice) {}
    public virtual bool swap(Model model) {}
    public virtual int transition_cost(ushort rcAttr, ushort lcAttr) {}
  }

  public class Node : IDisposable {
    protected bool swigCMemOwn;

    public float alpha { get; }
    public float beta { get; }
    public Node bnext { get; }
    public byte char_type { get; }
    public int cost { get; }
    public Node enext { get; }
    public string feature { get; }
    public uint id { get; }
    public byte isbest { get; }
    public ushort lcAttr { get; }
    public ushort length { get; }
    public Path lpath { get; }
    public Node next { get; }
    public ushort posid { get; }
    public Node prev { get; }
    public float prob { get; set; }
    public ushort rcAttr { get; }
    public ushort rlength { get; }
    public Path rpath { get; }
    public byte stat { get; }
    public string surface { get; }
    public short wcost { get; }

    protected virtual void Dispose(bool disposing) {}
    public void Dispose() {}
    ~Node() {}
  }

  public class Path : IDisposable {
    protected bool swigCMemOwn;

    public int cost { get; }
    public Path lnext { get; }
    public Node lnode { get; }
    public float prob { get; set; }
    public Path rnext { get; }
    public Node rnode { get; }

    protected virtual void Dispose(bool disposing) {}
    public void Dispose() {}
    ~Path() {}
  }

  public class Tagger : IDisposable {
    public static bool parse(Model model, Lattice lattice) {}
    public static string version() {}

    protected bool swigCMemOwn;

    public Tagger() {}
    public Tagger(string argc) {}

    protected virtual void Dispose(bool disposing) {}
    public void Dispose() {}
    ~Tagger() {}
    public virtual bool all_morphs() {}
    public virtual DictionaryInfo dictionary_info() {}
    public virtual string formatNode(Node node) {}
    public virtual int lattice_level() {}
    public virtual string next() {}
    public virtual Node nextNode() {}
    public virtual bool parse(Lattice lattice) {}
    public virtual string parse(string str) {}
    public virtual string parseNBest(uint N, string str) {}
    public virtual bool parseNBestInit(string str) {}
    public virtual Node parseToNode(string str) {}
    public string parseToString(string str) {}
    public string parseToString(string str, uint length) {}
    public virtual bool partial() {}
    public virtual int request_type() {}
    public virtual void set_all_morphs(bool all_morphs) {}
    public virtual void set_lattice_level(int level) {}
    public virtual void set_partial(bool @partial) {}
    public virtual void set_request_type(int request_type) {}
    public virtual void set_theta(float theta) {}
    public virtual float theta() {}
    public virtual string what() {}
  }
}

namespace Smdn.Text.Ondulish {
  public static class KanaUtils {
    public static string ConvertWideHiraganaToKatakana(string input) {}
    public static string ConvertWideKatakanaToHiragana(string input) {}
    public static string ConvertWideKatakanaToNarrowKatakana(string input) {}
  }

  public class Translator : IDisposable {
    public static Tagger CreateTaggerForBundledDictionary() {}

    public Translator() {}
    public Translator(Tagger tagger, bool shouldDisposeTagger) {}

    public IReadOnlyDictionary<string, string> PhraseDictionary { get; }
    public IReadOnlyDictionary<string, string> WordDictionary { get; }

    protected virtual void Dispose(bool disposing) {}
    public void Dispose() {}
    public string Translate(string input, bool convertKatakanaToNarrow = true) {}
    public void Translate(TextReader input, TextWriter output, bool convertKatakanaToNarrow = true) {}
    public void Translate(string input, TextWriter output, bool convertKatakanaToNarrow = true) {}
  }
}
