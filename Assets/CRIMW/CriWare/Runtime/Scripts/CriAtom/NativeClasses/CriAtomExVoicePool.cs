/****************************************************************************
 *
 * Copyright (c) 2011 CRI Middleware Co., Ltd.
 *
 ****************************************************************************/

#if !UNITY_WEBGL
	#define CRIATOMEX_SUPPORT_INSERTION_DSP
	#define CRIATOMEX_SUPPORT_STANDARD_VOICE_POOL
	#define CRIATOMEX_SUPPORT_RAW_PCM_VOICE_POOL
#elif UNITY_WEBGL && UNITY_EDITOR
	#define CRIATOMEX_SUPPORT_WAVE_VOICE_POOL
#endif
#if !(UNITY_WEBGL || UNITY_STADIA)
 	#define CRIATOMEX_SUPPORT_WAVE_VOICE_POOL
#endif

using System;
using System.Runtime.InteropServices;

/*==========================================================================
 *      CRI Atom Native Wrapper
 *=========================================================================*/
/**
 * \addtogroup CRIATOM_NATIVE_WRAPPER
 * @{
 */

namespace CriWare {

/**
 * <summary>ボイスプールの制御を行うための抽象クラス</summary>
 * <remarks>
 * <para header='説明'>ボイスプールの制御を行うための抽象クラスです。<br/>
 * 実際にボイスプールの作成を行う場合は、このクラスを継承した各コーデック/再生形式ごとのクラスを利用します。</para>
 * </remarks>
 */
public abstract class CriAtomExVoicePool : CriDisposable
{
	/* @cond DOXYGEN_IGNORE */
	public const int StandardMemoryAsrVoicePoolId       = 0;    /**< ASRによる標準メモリ再生ボイスプールID */
	public const int StandardStreamingAsrVoicePoolId    = 1;    /**< ASRによる標準ストリーミング再生ボイスプールをID */
	public const int StandardMemoryNsrVoicePoolId       = 2;    /**< NSRによる標準メモリ再生ボイスプールID */
	public const int StandardStreamingNsrVoicePoolId    = 3;    /**< NSRによる標準ストリーミング再生ボイスプールID */
	/* @endcond */

	/**
	 * <summary>プラグイン内部で生成するボイスプールへアクセスするためのID</summary>
	 * <seealso cref='CriAtomExVoicePool.GetNumUsedVoices'/>
	 */
	public enum VoicePoolId
	{
		/* 機種共通のボイスプールID */
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_IOS || UNITY_TVOS || UNITY_PS3 || UNITY_PS4 || UNITY_PS5 || UNITY_WINRT || UNITY_XBOXONE || UNITY_GAMECORE_XBOXONE || UNITY_GAMECORE_SCARLETT || UNITY_WEBGL || UNITY_SWITCH || UNITY_STANDALONE_LINUX || UNITY_OPENHARMONY
		StandardMemory          = StandardMemoryAsrVoicePoolId,     /**< 機種標準のメモリ再生ボイスプールID */
		StandardStreaming       = StandardStreamingAsrVoicePoolId,  /**< 機種標準のストリーミング再生ボイスプールID */
#elif UNITY_PSP2
		StandardMemory          = StandardMemoryNsrVoicePoolId,     /**< 機種標準のメモリ再生ボイスプールID */
		StandardStreaming       = StandardStreamingNsrVoicePoolId,  /**< 機種標準のストリーミング再生ボイスプールID */
#else
		#error unsupported platform
#endif
		HcaMxMemory             = 4,                                /**< HCA-MXメモリ再生ボイスプールID */
		HcaMxStreaming          = 5,                                /**< HCA-MXストリーミング再生ボイスプールID */

		/* 機種固有のボイスプールID */
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_TVOS || UNITY_PS3 || UNITY_WINRT || UNITY_XBOXONE || UNITY_GAMECORE_XBOXONE || UNITY_GAMECORE_SCARLETT || UNITY_WEBGL || UNITY_SWITCH || UNITY_STANDALONE_LINUX || UNITY_OPENHARMONY
#elif UNITY_ANDROID
		LowLatencyMemory        = StandardMemoryNsrVoicePoolId,     /**< [Android] 低遅延メモリ再生ボイスプールID */
		LowLatencyStreaming     = StandardStreamingNsrVoicePoolId,  /**< [Android] 低遅延ストリーミング再生ボイスプールID */
#else
#error unsupported platform
#endif
	}

	/**
	 * <summary>ピッチシフタDSP動作モード</summary>
	 * <remarks>
	 * <para header='説明'>ピッチシフトの処理方法（アルゴリズム）を指定します。</para>
	 * </remarks>
	 * <seealso cref='CriAtomExVoicePool.AttachDspPitchShifter'/>
	 * <seealso cref='CriAtomExPlayer.SetDspParameter'/>
	 */
	public enum PitchShifterMode : int {
		Music       = 0,
		Vocal       = 1,
		SoundEffect = 2,
		Speech      = 3
	};

	/**
	 * <summary>ボイスプールのボイス使用状況を表すための構造体</summary>
	 * <seealso cref='CriAtomExVoicePool.GetNumUsedVoices'/>
	 */
	public struct UsedVoicesInfo
	{
		public int numUsedVoices;   /**< 使用中のボイス数 */
		public int numPoolVoices;   /**< ボイスプールのボイス数 */
	}

	/**
	 * <summary>ボイスプールのボイス使用状況取得</summary>
	 * <param name='voicePoolId'>ボイスプールのID</param>
	 * <returns>ボイス使用状況</returns>
	 * <remarks>
	 * <para header='説明'>指定されたボイスプールのボイス使用状況を取得します。</para>
	 * <para header='注意'>本関数はデバッグ目的でのみ使用してください。</para>
	 * </remarks>
	 * <seealso cref='CriAtomExVoicePool::VoicePoolId'/>
	 * <seealso cref='CriAtomExVoicePool::UsedVoicesInfo'/>
	 */
	static public UsedVoicesInfo GetNumUsedVoices(VoicePoolId voicePoolId)
	{
		UsedVoicesInfo info;
		criAtomUnity_GetNumUsedVoices((int)voicePoolId, out info.numUsedVoices, out info.numPoolVoices);
		return info;
	}


	public IntPtr nativeHandle {get {return this._handle;} }

	/**
	 * <summary>ボイスプール識別子</summary>
	 * <remarks>
	 * <para header='説明'>ボイスプール識別子は、ボイスプールを一意に識別するためのIDです。<br/></para>
	 * <para header='備考'>プレーヤー（<see cref='CriAtomExPlayer.SetVoicePoolIdentifier'/>）とボイスプールの両方に同じ識別子を設定することで、<br/>
	 * 当該のプレーヤーは設定されたボイスプールからのみボイスを取得するようになります。</para>
	 * </remarks>
	 * <seealso cref='CriAtomExVoicePool::VoicePoolId'/>
	 * <seealso cref='CriAtomExVoicePool::UsedVoicesInfo'/>
	 */
	public uint identifier {get {return this._identifier;} }
	public int numVoices {get {return this._numVoices; } }
	public int maxChannels {get {return this._maxChannels; } }
	public int maxSamplingRate {get {return this._maxSamplingRate; } }

	/**
	 * <summary>ボイスプールの破棄</summary>
	 * <remarks>
	 * <para header='説明'>ボイスプールオブジェクトの破棄を行います。<br/>
	 * 作成したオブジェクトを本APIで破棄しない場合、リソースリークが発生しますので必ず破棄してください。</para>
	 * </remarks>
	 */
	public override void Dispose()
	{
		CriDisposableObjectManager.Unregister(this);
		if (this._handle != IntPtr.Zero) {
			CriAtomExVoicePool.criAtomExVoicePool_Free(this._handle);
			this._handle = IntPtr.Zero;
		}
		GC.SuppressFinalize(this);
	}

	/**
	 * <summary>ボイスプールのボイス使用状況取得</summary>
	 * <returns>ボイス使用状況</returns>
	 * <remarks>
	 * <para header='説明'>ボイス使用状況を取得します。</para>
	 * <para header='注意'>本関数はデバッグ目的でのみ使用してください。</para>
	 * </remarks>
	 * <seealso cref='CriAtomExVoicePool::UsedVoicesInfo'/>
	 */
	public UsedVoicesInfo GetNumUsedVoices()
	{
		UsedVoicesInfo info;
		if (this._handle != IntPtr.Zero) {
			criAtomExVoicePool_GetNumUsedVoices(this._handle, out info.numUsedVoices, out info.numPoolVoices);
		} else {
			info = new UsedVoicesInfo();
		}
		return info;
	}

#if CRIATOMEX_SUPPORT_INSERTION_DSP
	/**
	 * <summary>タイムストレッチDSPのアタッチ</summary>
	 * <remarks>
	 * <para header='説明'>ボイスプールにタイムストレッチDSPを追加します。</para>
	 * <para header='注意'>本関数は完了復帰型の関数です。<br/>
	 * 本関数を実行すると、しばらくの間Atomライブラリのサーバ処理がブロックされます。<br/>
	 * 音声再生中に本関数を実行すると、音途切れ等の不具合が発生する可能性があるため、 本関数の呼び出しは
	 * シーンの切り替わり等、負荷変動を許容できるタイミングで行ってください。</para>
	 * </remarks>
	 * <seealso cref='CriAtomExVoicePool::DetachInsertionDsp'/>
	 */
	public void AttachDspTimeStretch()
	{
		if (this._handle == IntPtr.Zero)
			return;

		ExTimeStretchConfig config;
		config.numDsp = this._numVoices;
		config.maxChannels = this._maxChannels;
		config.maxSamplingRate = this._maxSamplingRate;
		config.config.reserved = 0;
		criAtomExVoicePool_AttachDspTimeStretch(this._handle, ref config, IntPtr.Zero, 0);
	}

	/**
	 * <summary>ピッチシフタDSPのアタッチ</summary>
	 * <param name='mode'>ピッチシフトモード</param>
	 * <param name='windosSize'>ウィンドウサイズ</param>
	 * <param name='overlapTimes'>オーバーラップ回数</param>
	 * <remarks>
	 * <para header='説明'>ボイスプールにピッチシフタDSPを追加します。</para>
	 * <para header='注意'>本関数は完了復帰型の関数です。<br/>
	 * 本関数を実行すると、しばらくの間Atomライブラリのサーバ処理がブロックされます。<br/>
	 * 音声再生中に本関数を実行すると、音途切れ等の不具合が発生する可能性があるため、 本関数の呼び出しは
	 * シーンの切り替わり等、負荷変動を許容できるタイミングで行ってください。</para>
	 * </remarks>
	 * <seealso cref='CriAtomExVoicePool::DetachInsertionDsp'/>
	 */
	public void AttachDspPitchShifter(PitchShifterMode mode = PitchShifterMode.Music, int windosSize = 1024, int overlapTimes = 4)
	{
		if (this._handle == IntPtr.Zero)
			return;

		ExPitchShifterConfig config;
		config.numDsp = this._numVoices;
		config.maxChannels = this._maxChannels;
		config.maxSamplingRate = this._maxSamplingRate;
		config.config.mode = (int)mode;
		config.config.windowSize = windosSize;
		config.config.overlapTimes = overlapTimes;
		criAtomExVoicePool_AttachDspPitchShifter(this._handle, ref config, IntPtr.Zero, 0);
	}

	/**
	 * <summary>DSPのデタッチ</summary>
	 * <remarks>
	 * <para header='説明'>ボイスプールに追加したDSPを取り外します。</para>
	 * <para header='注意'>本関数は完了復帰型の関数です。<br/>
	 * 本関数を実行すると、しばらくの間Atomライブラリのサーバ処理がブロックされます。<br/>
	 * 音声再生中に本関数を実行すると、音途切れ等の不具合が発生する可能性があるため、 本関数の呼び出しは
	 * シーンの切り替わり等、負荷変動を許容できるタイミングで行ってください。</para>
	 * </remarks>
	 * <seealso cref='CriAtomExVoicePool::AttachDspPitchShifter'/>
	 * <seealso cref='CriAtomExVoicePool::AttachDspTimeStretch'/>
	 */
	public void DetachDsp()
	{
		if (this._handle == IntPtr.Zero)
			return;

		criAtomExVoicePool_DetachDsp(this._handle);
	}
#endif

	#region Internal Members

	~CriAtomExVoicePool()
	{
		Dispose();
	}

	/**
	 * <summary>ボイス駆動動作仕様コンフィグ構造体</summary>
	 * <remarks>
	 * <para header='説明'>ボイスの駆動動作仕様を指定するための構造体です。<br/>
	 * 本構造体で指定された設定に応じて、アンマネージメモリを必要なだけ確保します。<br/></para>
	 * </remarks>
	 */
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct PlayerConfig
	{
		/**
		 * <summary>最大出力チャンネル数</summary>
		 * <remarks>
		 * <para header='説明'>ボイスの駆動動作仕様を指定するための構造体です。<br/>
		 * 本構造体で指定された設定に応じて、リソースを必要なだけ確保します。<br/>
		 * 本項目で指定したチャンネル数"以下の"音声データを再生可能です。<br/></para>
		 * <para header='注意'>指定された最大出力チャンネル数を超えるデータは、再生することはできません。<br/>
		 * 例えば最大出力チャンネル数を1に設定した場合、ステレオ音声を再生することはできません。<br/>
		 * （モノラルにダウンミックスされて出力されることはありません。）<br/></para>
		 * </remarks>
		 */
		public int maxChannels;

		/**
		 * <summary>最大サンプリングレート</summary>
		 * <remarks>
		 * <para header='説明'>再生する音声のサンプリングレートを指定します。<br/>
		 * 本項目で指定したサンプリングレート"以下の"音声データを再生可能です。<br/>
		 * 通常、作成するボイスプールで再生予定のキューに含まれるマテリアルの最大サンプリングレートを指定します。<br/></para>
		 * <para header='備考'>最大サンプリングレートを下げることで、必要となるアンマネージメモリのサイズを抑えることが可能です。<br/>
		 * ただしピッチ変化を行う場合は、ピッチの最大変化分の最大サンプリングレートを指定します。<br/>
		 * タイムストレッチ機能を使用する場合は最大サンプリングレートをx2倍に増やす必要があります。<br/></para>
		 * <para header='注意'>指定された最大サンプリングレートを超えるデータは、再生することはできません。<br/>
		 * 例えば最大サンプリングレートを24000に設定した場合、48000Hzの音声を再生することはできません。<br/>
		 * （ダウンサンプリングされて出力されることはありません。）<br/></para>
		 * </remarks>
		 */
		public int maxSamplingRate;

		/**
		 * <summary>ストリーミング再生を行うかどうか</summary>
		 * <remarks>
		 * <para header='説明'>false を指定した場合、メモリ再生ができるボイスプールが作成されます。<br/>
		 * true を指定した場合、メモリ再生に加えてストリーミング再生ができるボイスプールが作成されます。<br/></para>
		 * <para header='備考'>trueにした場合、作成時にファイル読み込み用のリソースが確保されます。<br/>
		 * そのため、falseの場合に比べ、作成に必要なアンマネージメモリサイズが大きくなります。<br/></para>
		 * </remarks>
		 */
		public bool streamingFlag;

		/**
		 * <summary>サウンドレンダラタイプ</summary>
		 * <remarks>
		 * <para header='説明'>使用するサウンドレンダラの種別を指定します。<br/>
		 * <see cref='CriAtomEx.SoundRendererType.Default'/> を指定した場合、
		 * 音声データはデフォルト設定のサウンドレンダラに転送されます。<br/>
		 * <see cref='CriAtomEx.SoundRendererType.Native'/> を指定した場合、
		 * 音声データは各プラットフォームのサウンド出力に転送されます。<br/>
		 * <see cref='CriAtomEx.SoundRendererType.Asr'/> を指定した場合、
		 * 音声データはASR（Atom Sound Renderer）に転送されます。<br/>
		 * （ASRの出力先は、ASR初期化時に別途指定。）</para>
		 * </remarks>
		 */
		public int soundRendererType;

		/**
		 * <summary>デコード処理のレイテンシ</summary>
		 * <remarks>
		 * <para header='説明'>デコード処理のレイテンシを指定します。<br/>
		 * 0 に設定した場合、音声再生開始時に音声データのデコードを可能な限り遅延なく行います。<br/>
		 * （オンメモリ再生時は最初のサーバー処理内で再生開始に必要な量のデータをデコードし、音声の出力を開始します。）<br/>
		 * これに対し 1 以上に設定した場合、再生開始に必要なデータのデコードを、複数回のサーバー処理に分割して行います。<br/>
		 * （オンメモリ再生時であっても指定した回数サーバー処理が動作するまでは音声の出力が開始されません。）<br/></para>
		 * <para header='備考'>サウンドバッファー内のデータ残量を元に、サーバー処理当たりの音声データのデコード量を決定しています。<br/>
		 * 音声再生開始前はサウンドバッファーが空の状態のため、<br/>
		 * 音声再生中に比べて多くのデータ（再生中の 2 ～ 4 倍程度）がデコードされます。<br/>
		 * プレーヤー当たりの音声データのデコード処理負荷は小さいため、<br/>
		 * 一音一音の発音開始時の処理負荷が問題になることはほとんどありません。<br/>
		 * しかし、アプリケーション中で 1V に大量の発音リクエストを同時に発行した場合、
		 * 全てのプレーヤーの処理負荷のピークが同期し、負荷が目に見えて大きくなる場合があります。<br/>
		 * こういった制御を行うケースでは、本値を増やすことで、<br/>
		 * 局所的に処理負荷が高くなる症状を回避することが可能です。<br/>
		 * <br/>
		 * デフォルト値は、ほとんどの環境で 0 に設定されています。<br/>
		 * しかし、携帯ゲーム機等、わずかな負荷変動でもアプリケーションに大きな影響を及ぼす環境では、
		 * デフォルト値が 1 以上に設定されている可能性があります。<br/>
		 * （実際にセットされる値については構造体をデフォルト値で取得し確認してください）<br/>
		 * <br/>
		 * 現状 4 以上の値を指定することはできません。<br/>
		 * （4 以上の値を指定した場合でも、ライブラリ内で 3 に変更されます。）<br/></para>
		 * </remarks>
		 */
		public int decodeLatency;
		private IntPtr context;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	private struct PitchShifterConfig {
		public int mode;
		public int windowSize;
		public int overlapTimes;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	private struct ExPitchShifterConfig {
		public int numDsp;
		public int maxChannels;
		public int maxSamplingRate;
		public PitchShifterConfig config;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	private struct TimeStretchConfig {
		public int reserved;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	private struct ExTimeStretchConfig {
		public int numDsp;
		public int maxChannels;
		public int maxSamplingRate;
		public TimeStretchConfig config;
	}

	protected IntPtr _handle = IntPtr.Zero;
	protected uint _identifier = 0;
	protected int _numVoices = 0;
	protected int _maxChannels = 0;
	protected int _maxSamplingRate = 0;

	#endregion

	#region DLL Import
	#if !CRIWARE_ENABLE_HEADLESS_MODE
	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	private static extern void criAtomUnity_GetNumUsedVoices(int voice_pool_id, out int num_used_voices, out int num_pool_voices);

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	private static extern void criAtomExVoicePool_GetNumUsedVoices(IntPtr pool, out int num_used_voices, out int num_pool_voices);

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	public static extern void criAtomExVoicePool_Free(IntPtr pool);

	#if CRIATOMEX_SUPPORT_INSERTION_DSP
	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	private static extern void criAtomExVoicePool_AttachDspTimeStretch(IntPtr pool, ref ExTimeStretchConfig config, IntPtr work, int work_size) ;

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	private static extern void criAtomExVoicePool_AttachDspPitchShifter(IntPtr pool, ref ExPitchShifterConfig config, IntPtr work, int work_size) ;

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	private static extern void criAtomExVoicePool_DetachDsp(IntPtr pool);
	#endif
	#else
	private static void criAtomUnity_GetNumUsedVoices(int voice_pool_id, out int num_used_voices, out int num_pool_voices)
		{ num_used_voices = num_pool_voices = 0; }
	private static void criAtomExVoicePool_GetNumUsedVoices(IntPtr pool, out int num_used_voices, out int num_pool_voices)
		{ num_used_voices = num_pool_voices = 0; }
	public static void criAtomExVoicePool_Free(IntPtr pool) { }
	#if CRIATOMEX_SUPPORT_INSERTION_DSP
	private static void criAtomExVoicePool_AttachDspTimeStretch(IntPtr pool, ref ExTimeStretchConfig config, IntPtr work, int work_size) { }
	private static void criAtomExVoicePool_AttachDspPitchShifter(IntPtr pool, ref ExPitchShifterConfig config, IntPtr work, int work_size) { }
	private static void criAtomExVoicePool_DetachDsp(IntPtr pool) { }
	#endif
	#endif
	#endregion
}

#if CRIATOMEX_SUPPORT_STANDARD_VOICE_POOL

/**
 * <summary>標準ボイスプール</summary>
 */
public class CriAtomExStandardVoicePool: CriAtomExVoicePool
{
	/**
	 * <summary>標準ボイスプール初期化コンフィグ</summary>
	 */
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct Config {
		/**
		 * <summary>ボイスプール識別子</summary>
		 * <remarks>
		 * <para header='説明'>ボイスプールを特定するための一意の数値です。<br/>
		 * ボイスプールを作成する際にボイスプール識別子を指定すると、<br/>
		 * そのボイスプールは識別子を知っているプレーヤーからのみ参照されます。<br/></para>
		 * <para header='備考'>複数のボイスプールに同一のボイスプール識別子を指定することも可能です。</para>
		 * </remarks>
		 */
		public uint identifier;

		/** <summary>プールするボイスの数</summary> */
		public int numVoices;

		/** <summary>ボイスの仕様</summary> */
		public PlayerConfig playerConfig;

		/** <summary>ストリーム再生専用かどうか</summary> */
		public bool isStreamingOnly;

		/**
		 * <summary>最小チャンネル数</summary>
		 * <remarks>
		 * <para header='説明'>作成されたボイスプールは、最小チャンネル数以上かつ最大チャンネル数(<see cref='PlayerConfig.maxChannels'/>)<br/>
		 * 以下の音声を再生する場合にのみ使用されます。</para>
		 * </remarks>
		 */
		public System.Int32 minChannels;

		/**
		 * <summary>標準ボイスプール作成用コンフィグ構造体のデフォルト値</summary>
		 */
		public static Config Default() {
			Config config = new Config();
			SetDefaultConfigForStandardVoicePool(ref config);
			return config;
		}
	}

	/**
	 * <summary>標準ボイスプール作成用コンフィグ構造体にデフォルト値をセット</summary>
	 */
	public static void SetDefaultConfigForStandardVoicePool(ref Config config) {
		criAtomExVoicePool_SetDefaultConfigForStandardVoicePool_Macro(ref config);
	}

	/**
	 * <summary>追加標準ボイスプールの作成</summary>
	 * <param name='config'>標準ボイスプール初期化コンフィグ</param>
	 * <returns>標準ボイスプール</returns>
	 * <remarks>
	 * <para header='説明'>標準ボイスプールを追加で作成します。<br/>
	 * 6 チャンネル以上の音声を再生する場合や追加のボイスプールが必要な場合は、本 API でボイスプールを作成してください。<br/></para>
	 * <para header='注意'>再生終了後は、必ず、Dispose 関数でオブジェクトを破棄してください。<br/>
	 * 特定の CriAtomExPlayer に対して、作成したボイスプールからボイスを取得するように明示的に設定したい場合は、<br/>
	 * identifier としてデフォルトの 0 以外の値を指定して作成し、 CriAtomExPlayer::SetVoicePoolIdentifier 関数を<br/>
	 * 呼び出してください。<br/></para>
	 * </remarks>
	 * <seealso cref='CriAtomExPlayer::SetVoicePoolIdentifier'/>
	 */
	public CriAtomExStandardVoicePool(Config config)
	{
		this._identifier = config.identifier;
		this._numVoices = config.numVoices;
		this._maxChannels = config.playerConfig.maxChannels;
		this._maxSamplingRate = config.playerConfig.maxSamplingRate;

		this._handle = criAtomExVoicePool_AllocateStandardVoicePool(ref config, IntPtr.Zero, 0);

		CriDisposableObjectManager.Register(this, CriDisposableObjectManager.ModuleType.Atom);
	}

	/**
	 * <summary>追加標準ボイスプールの作成</summary>
	 * <param name='numVoices'>ボイス数</param>
	 * <param name='maxChannels'>最大チャンネル数</param>
	 * <param name='maxSamplingRate'>最大サンプリングレート</param>
	 * <param name='streamingFlag'>ストリーミング再生フラグ</param>
	 * <param name='identifier'>ボイスプール識別子</param>
	 * <returns>標準ボイスプール</returns>
	 * <remarks>
	 * <para header='説明'>標準ボイスプールを追加で作成します。<br/>
	 * 6 チャンネル以上の音声を再生する場合、本 API でボイスプールを作成してください。<br/>
	 * streamingFlag に false を指定した場合、メモリ再生ができるボイスプールが作成されます。<br/>
	 * streamingFlag に true を指定した場合、メモリ再生に加えてストリーミング再生ができるボイスプールが作成されます。<br/>
	 * maxSamplingRate には、作成するボイスプールで再生予定のキューに含まれるマテリアルの最大サンプリングレートを指定します。<br/>
	 * ただしピッチ変化を行う場合は、ピッチの最大変化分の最大サンプリングレートを指定します。<br/>
	 * タイムストレッチ機能を使用する場合は最大サンプリングレートをx2倍に増やす必要があります。<br/></para>
	 * <para header='注意'>再生終了後は、必ず、Dispose 関数でオブジェクトを破棄してください。<br/>
	 * 特定の CriAtomExPlayer に対して、作成したボイスプールからボイスを取得するように明示的に設定したい場合は、<br/>
	 * identifier としてデフォルトの 0 以外の値を指定して作成し、 CriAtomExPlayer::SetVoicePoolIdentifier 関数を<br/>
	 * 呼び出してください。<br/></para>
	 * </remarks>
	 * <seealso cref='CriAtomExPlayer::SetVoicePoolIdentifier'/>
	 */
	public CriAtomExStandardVoicePool(int numVoices, int maxChannels, int maxSamplingRate, bool streamingFlag, uint identifier = 0)
	{
		this._identifier = identifier;
		this._numVoices = numVoices;
		this._maxChannels = maxChannels;
		this._maxSamplingRate = maxSamplingRate;

		Config config = new Config();
		config.identifier = identifier;
		config.numVoices = numVoices;
		config.playerConfig.maxChannels = maxChannels;
		config.playerConfig.maxSamplingRate = maxSamplingRate;
		config.playerConfig.streamingFlag = streamingFlag;
		config.playerConfig.soundRendererType = (int)CriAtomEx.SoundRendererType.Asr;
		config.playerConfig.decodeLatency = 0;
		config.isStreamingOnly = false;
		this._handle = criAtomExVoicePool_AllocateStandardVoicePool(ref config, IntPtr.Zero, 0);
		if (this._handle == IntPtr.Zero) {
			throw new Exception("CriAtomExStandardVoicePool() failed.");
		}

		CriDisposableObjectManager.Register(this, CriDisposableObjectManager.ModuleType.Atom);
	}

	#region DLL Import
	#if !CRIWARE_ENABLE_HEADLESS_MODE
	[DllImport(CriWare.Common.pluginName)]
	private static extern IntPtr criAtomExVoicePool_AllocateStandardVoicePool(ref Config config, IntPtr work, int work_size);

	[DllImport(CriWare.Common.pluginName)]
	private static extern void criAtomExVoicePool_SetDefaultConfigForStandardVoicePool_Macro(ref Config config);
	#else
	private static IntPtr criAtomExVoicePool_AllocateStandardVoicePool(ref Config config, IntPtr work, int work_size) { return new IntPtr(1); }
	private static void criAtomExVoicePool_SetDefaultConfigForStandardVoicePool_Macro(ref Config config) { }
	#endif
	#endregion
}

#endif

#if CRIATOMEX_SUPPORT_WAVE_VOICE_POOL

/**
 * <summary>Wave ボイスプール</summary>
 */
public class CriAtomExWaveVoicePool: CriAtomExVoicePool
{
	/**
	 * <summary>Waveボイスプール初期化コンフィグ</summary>
	 */
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct Config {
		/**
		 * <summary>ボイスプール識別子</summary>
		 * <remarks>
		 * <para header='説明'>ボイスプールを特定するための一意の数値です。<br/>
		 * ボイスプールを作成する際にボイスプール識別子を指定すると、<br/>
		 * そのボイスプールは識別子を知っているプレーヤーからのみ参照されます。<br/></para>
		 * <para header='備考'>複数のボイスプールに同一のボイスプール識別子を指定することも可能です。</para>
		 * </remarks>
		 */
		public uint identifier;

		/** <summary>プールするボイスの数</summary> */
		public int numVoices;

		/** <summary>ボイスの仕様</summary> */
		public PlayerConfig playerConfig;

		/** <summary>ストリーム再生専用かどうか</summary> */
		public bool isStreamingOnly;

		/**
		 * <summary>最小チャンネル数</summary>
		 * <remarks>
		 * <para header='説明'>作成されたボイスプールは、最小チャンネル数以上かつ最大チャンネル数(<see cref='PlayerConfig.maxChannels'/>)<br/>
		 * 以下の音声を再生する場合にのみ使用されます。</para>
		 * </remarks>
		 */
		public System.Int32 minChannels;

		/**
		 * <summary>Waveボイスプール作成用コンフィグ構造体のデフォルト値</summary>
		 */
		public static Config Default() {
			Config config = new Config();
			SetDefaultConfigForWaveVoicePool(ref config);
			return config;
		}
	}

	/**
	 * <summary>Waveボイスプール作成用コンフィグ構造体にデフォルト値をセット</summary>
	 */
	public static void SetDefaultConfigForWaveVoicePool(ref Config config) {
		criAtomExVoicePool_SetDefaultConfigForWaveVoicePool_Macro(ref config);
	}

	/**
	 * <summary>Wave 再生可能なボイスプールの作成</summary>
	 * <param name='config'>Waveボイスプール初期化コンフィグ</param>
	 * <returns>Waveボイスプール</returns>
	 * <remarks>
	 * <para header='説明'>本関数を実行することで、Wave 再生が可能なボイスがプールされます。<br/>
	 * AtomEx プレーヤで Wave データ（もしくは Wave データを含むキュー）の再生を行うと、
	 * AtomEx プレーヤは作成された Wave ボイスプールからボイスを取得し、再生を行います。<br/>
	 * 再生終了後は、必ず、Dispose 関数でオブジェクトを破棄してください。<br/></para>
	 * <para header='注意'>再生終了後は、必ず、Dispose 関数でオブジェクトを破棄してください。<br/>
	 * 特定の CriAtomExPlayer に対して、作成したボイスプールからボイスを取得するように明示的に設定したい場合は、<br/>
	 * identifier としてデフォルトの 0 以外の値を指定して作成し、 CriAtomExPlayer::SetVoicePoolIdentifier 関数を<br/>
	 * 呼び出してください。<br/></para>
	 * </remarks>
	 * <seealso cref='CriAtomExPlayer::SetVoicePoolIdentifier'/>
	 */
	public CriAtomExWaveVoicePool(Config config)
	{
		this._identifier = config.identifier;
		this._numVoices = config.numVoices;
		this._maxChannels = config.playerConfig.maxChannels;
		this._maxSamplingRate = config.playerConfig.maxSamplingRate;

		this._handle = criAtomExVoicePool_AllocateWaveVoicePool(ref config, IntPtr.Zero, 0);

		CriDisposableObjectManager.Register(this, CriDisposableObjectManager.ModuleType.Atom);
	}

	/**
	 * <summary>Wave ボイスプールの作成</summary>
	 * <param name='numVoices'>ボイス数</param>
	 * <param name='maxChannels'>最大チャンネル数</param>
	 * <param name='maxSamplingRate'>最大サンプリングレート</param>
	 * <param name='streamingFlag'>ストリーミング再生フラグ</param>
	 * <param name='identifier'>ボイスプール識別子</param>
	 * <returns>Wave ボイスプール</returns>
	 * <remarks>
	 * <para header='説明'>本関数を実行することで、Wave 再生が可能なボイスがプールされます。<br/>
	 * AtomEx プレーヤで Wave データ（もしくは Wave データを含むキュー）の再生を行うと、
	 * AtomEx プレーヤは作成された Wave ボイスプールからボイスを取得し、再生を行います。<br/>
	 * 再生終了後は、必ず、Dispose 関数でオブジェクトを破棄してください。<br/>
	 * 特定の CriAtomExPlayer に対して、作成したボイスプールからボイスを取得するように明示的に設定したい場合は、
	 * identifier としてデフォルトの 0 以外の値を指定して作成し、 CriAtomExPlayer::SetVoicePoolIdentifier 関数を
	 * 呼び出してください。</para>
	 * </remarks>
	 * <seealso cref='CriAtomExPlayer::SetVoicePoolIdentifier'/>
	 */
	public CriAtomExWaveVoicePool(int numVoices, int maxChannels, int maxSamplingRate, bool streamingFlag, uint identifier = 0)
	{
		this._identifier = identifier;
		this._numVoices = numVoices;
		this._maxChannels = maxChannels;
		this._maxSamplingRate = maxSamplingRate;

		Config config = new Config();
		config.identifier = identifier;
		config.numVoices = numVoices;
		config.playerConfig.maxChannels = maxChannels;
		config.playerConfig.maxSamplingRate = maxSamplingRate;
		config.playerConfig.streamingFlag = streamingFlag;
		config.playerConfig.soundRendererType = (int)CriAtomEx.SoundRendererType.Asr;
		config.playerConfig.decodeLatency = 0;
		config.isStreamingOnly = false;
		config.minChannels = 1;
		this._handle = criAtomExVoicePool_AllocateWaveVoicePool(ref config, IntPtr.Zero, 0);
		if (this._handle == IntPtr.Zero) {
			throw new Exception("CriAtomExWaveVoicePool() failed.");
		}

		CriDisposableObjectManager.Register(this, CriDisposableObjectManager.ModuleType.Atom);
	}

	#region DLL Import
	#if !CRIWARE_ENABLE_HEADLESS_MODE
	[DllImport(CriWare.Common.pluginName)]
	private static extern IntPtr criAtomExVoicePool_AllocateWaveVoicePool(ref Config config, IntPtr work, int work_size);

	[DllImport(CriWare.Common.pluginName)]
	private static extern void criAtomExVoicePool_SetDefaultConfigForWaveVoicePool_Macro(ref Config config);
	#else
	private static IntPtr criAtomExVoicePool_AllocateWaveVoicePool(ref Config config, IntPtr work, int work_size) { return IntPtr.Zero; }
	private static void criAtomExVoicePool_SetDefaultConfigForWaveVoicePool_Macro(ref Config config) { }
	#endif
	#endregion
}

#endif

#if  CRIATOMEX_SUPPORT_RAW_PCM_VOICE_POOL

/**
 * <summary>RawPCM ボイスプール</summary>
 */
public class CriAtomExRawPcmVoicePool: CriAtomExVoicePool
{
	/**
	 * <summary>RawPCM フォーマット</summary>
	 * <remarks>
	 * <para header='説明'>RawPCM ボイスプールを用いて再生するデータフォーマットです。</para>
	 * </remarks>
	 * <seealso cref='CriAtomExVoicePool.CriAtomExRawPcmVoicePool'/>
	 */
	public enum RawPcmFormat {
		Sint16  = 0,
		Float32 = 1
	}

	/**
	 * <summary>RawPCM ボイスプールの作成</summary>
	 * <param name='numVoices'>ボイス数</param>
	 * <param name='maxChannels'>最大チャンネル数</param>
	 * <param name='maxSamplingRate'>最大サンプリングレート</param>
	 * <param name='format'>RawPCM フォーマット</param>
	 * <param name='identifier'>ボイスプール識別子</param>
	 * <returns>RawPCM ボイスプール</returns>
	 * <remarks>
	 * <para header='説明'>本関数を実行することで、RawPCM 再生が可能なボイスがプールされます。<br/>
	 * AtomEx プレーヤで RawPCM データ（もしくは RawPCM データを含むキュー）の再生を行うと、
	 * AtomEx プレーヤは作成された RawPCM ボイスプールからボイスを取得し、再生を行います。<br/>
	 * 再生終了後は、必ず、Dispose 関数でオブジェクトを破棄してください。<br/>
	 * 特定の CriAtomExPlayer に対して、作成したボイスプールからボイスを取得するように明示的に設定したい場合は、
	 * identifier としてデフォルトの 0 以外の値を指定して作成し、 CriAtomExPlayer::SetVoicePoolIdentifier 関数を
	 * 呼び出してください。</para>
	 * <para header='注意'>現状、一度作成した RawPCM ボイスプールのフォーマットを後から変更することはできません。<br/>
	 * 再生するデータに合わせたフォーマットを指定して作成を行うようにしてください。<br/>
	 * また、 CriWare.CriAtomExPlayer に対してチャンネル数やサンプリングレートを指定しても、
	 * ボイスプールに対して設定したフォーマットに基づいて再生が行われます。</para>
	 * </remarks>
	 * <seealso cref='CriAtomExPlayer::SetVoicePoolIdentifier'/>
	 */
	public CriAtomExRawPcmVoicePool(int numVoices, int maxChannels, int maxSamplingRate, RawPcmFormat format, uint identifier = 0)
	{
		this._identifier = identifier;
		this._numVoices = numVoices;
		this._maxChannels = maxChannels;
		this._maxSamplingRate = maxSamplingRate;

		RawPcmVoicePoolConfig config = new RawPcmVoicePoolConfig();
		config.identifier = identifier;
		config.numVoices = numVoices;
		config.playerConfig.maxChannels = maxChannels;
		config.playerConfig.maxSamplingRate = maxSamplingRate;
		config.playerConfig.format = format;
		config.playerConfig.soundRendererType = (int)CriAtomEx.SoundRendererType.Asr;
		config.playerConfig.decodeLatency = 0;
		this._handle = criAtomExVoicePool_AllocateRawPcmVoicePool(ref config, IntPtr.Zero, 0);
		if (this._handle == IntPtr.Zero) {
			throw new Exception("CriAtomExRawPcmVoicePool() failed.");
		}

		CriDisposableObjectManager.Register(this, CriDisposableObjectManager.ModuleType.Atom);
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	protected struct RawPcmPlayerConfig {
		public RawPcmFormat format;
		public int maxChannels;
		public int maxSamplingRate;
		public int soundRendererType;
		public int decodeLatency;
		private IntPtr context;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	protected struct RawPcmVoicePoolConfig {
		public uint identifier;
		public int numVoices;
		public RawPcmPlayerConfig playerConfig;
	}

	#region DLL Import
#if !CRIWARE_ENABLE_HEADLESS_MODE
	[DllImport(CriWare.Common.pluginName)]
	private static extern IntPtr criAtomExVoicePool_AllocateRawPcmVoicePool(ref RawPcmVoicePoolConfig config, IntPtr work, int work_size);
#else
	private static IntPtr criAtomExVoicePool_AllocateRawPcmVoicePool(ref RawPcmVoicePoolConfig config, IntPtr work, int work_size) { return new IntPtr(1); }
#endif
	#endregion
}

#endif

} //namespace CriWare
/**
 * @}
 */

/* --- end of file --- */
