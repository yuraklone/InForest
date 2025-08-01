﻿/****************************************************************************
 *
 * Copyright (c) 2011 CRI Middleware Co., Ltd.
 *
 ****************************************************************************/

/*---------------------------
 * Force Load Data with Async Method Defines
 *---------------------------*/
#if UNITY_WEBGL
	#define CRIWARE_FORCE_LOAD_ASYNC
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
#if !UNITY_EDITOR && (UNITY_WINRT && !ENABLE_IL2CPP)
using System.Reflection;
#endif

/*==========================================================================
 *      CRI Atom Unity Integration
 *=========================================================================*/

/**
 * \addtogroup CRIATOM_UNITY_INTEGRATION
 * @{
 */

namespace CriWare {

/**
 * <summary>CRIAtomライブラリのグローバルクラスです。</summary>
 * <remarks>
 * <para header='説明'>CRIAtomライブラリの初期化関数や、ライブラリ内で共有する変数型を含むクラスです。<br/></para>
 * </remarks>
 */
public static class CriAtomPlugin
{
	#region Editor/Runtime共通

#if UNITY_EDITOR
	public static bool showDebugLog = false;
	public delegate void PreviewCallback();
	public static PreviewCallback previewCallback = null;
#endif

	public static void Log(string log)
	{
	#if UNITY_EDITOR
		if (CriAtomPlugin.showDebugLog) {
			Debug.Log(log);
		}
	#endif
	}

	/* 初期化カウンタ */
	private static int initializationCount = 0;

	public static bool isInitialized { get { return initializationCount > 0; } }
	
	/**
	 * <summary>Atomライブラリ初期化直前のイベント</summary>
	 * <remarks>
	 * <para header='説明'>Atomライブラリの初期化直前に呼び出されるコールバックイベントです。<br/>
	 * ライブラリ初期化前に呼び出された後、内容がクリアされません。</para>
	 * </remarks>
	 */
	public static event System.Action OnBeforeInitialize = null;

	/**
	 * <summary>Atomライブラリ初期化直後のイベント</summary>
	 * <remarks>
	 * <para header='説明'>Atomライブラリの初期化直後に呼び出されるコールバックイベントです。<br/>
	 * ライブラリ初期化後に呼び出された後に、内容がクリアされます。</para>
	 * </remarks>
	 */
	public static event System.Action OnInitialized = null;

	/**
	 * <summary>Atomライブラリ終了直前のイベント</summary>
	 * <remarks>
	 * <para header='説明'>Atomライブラリの終了直前に呼び出されるコールバックイベントです。<br/>
	 * ライブラリ終了前に呼び出された後、内容がクリアされません。</para>
	 * </remarks>
	 */
	public static event System.Action OnBeforeFinalize = null;

	/**
	 * <summary>Atomライブラリ終了直後のイベント</summary>
	 * <remarks>
	 * <para header='説明'>Atomライブラリの終了直後に呼び出されるコールバックイベントです。<br/>
	 * ライブラリ終了後に呼び出された後に、内容がクリアされます。</para>
	 * </remarks>
	 */
	public static event System.Action OnFinalized = null;

	/**
	 * <summary>キューリンクコールバックの実行</summary>
	 * <remarks>
	 * <para header='説明'>Atom サーバースレッドにてトリガーされたキューリンクコールバックイベントを実行するための関数です。<br/>
	 * 本関数を呼び出すと、イベントが発生していた場合は CriAtomEx::OnCueLinkCallback に登録されたコールバック関数が実行されます。</para>
	 * <para header='注意'>本関数は CriAtomServer コンポーネントにより定期的に呼び出されるため、通常はユーザーが呼び出す必要はありません。<br/>
	 * Editor 拡張などのアプリケーション実行時以外にコールバックを動作させたい場合には、本関数をよびだしてください。</para>
	 * </remarks>
	 */
	public static void ExecuteQueuedCueLinkCallbacks()
	{
		if (CriAtom.HasCueLinkCallback){
			criAtomUnity_ExecuteQueuedCueLinkCallbacks();
		}
	}

	/**
	 * <summary>シーケンスイベントコールバックの実行</summary>
	 * <remarks>
	 * <para header='説明'>Atom サーバースレッドにてトリガーされたコールバックマーカー同期のコールバックイベントを実行するための関数です。<br/>
	 * 本関数を呼び出すと、イベントが発生していた場合は CriAtomExSequencer::OnCallback に登録されたコールバック関数が実行されます。</para>
	 * <para header='注意'>本関数は CriAtomServer コンポーネントにより定期的に呼び出されるため、通常はユーザーが呼び出す必要はありません。<br/>
	 * Editor 拡張などのアプリケーション実行時以外にコールバックを動作させたい場合には、本関数をよびだしてください。</para>
	 * </remarks>
	 */
	public static void ExecuteQueuedEventCallbacks()
	{
		if (CriAtom.HasUserCallback){
			criAtomUnitySequencer_ExecuteQueuedEventCallbacks();
		}
	}

	/**
	 * <summary>ビート同期コールバックの実行</summary>
	 * <remarks>
	 * <para header='説明'>Atom サーバースレッドにてトリガーされたビート同期コールバックイベントを実行するための関数です。<br/>
	 * 本関数を呼び出すと、イベントが発生していた場合は CriAtomExPlayer::OnBeatSyncCallback および CriAtomExBeatSync::OnCallback に登録されたコールバック関数が実行されます。</para>
	 * <para header='注意'>本関数は CriAtomServer コンポーネントにより定期的に呼び出されるため、通常はユーザーが呼び出す必要はありません。<br/>
	 * Editor 拡張などのアプリケーション実行時以外にコールバックを動作させたい場合には、本関数をよびだしてください。</para>
	 * </remarks>
	 */
	public static void ExecuteQueuedBeatSyncCallbacks()
	{
		if (CriAtom.HasBeatSyncCallback){
			criAtomUnity_ExecuteQueuedBeatSyncCallbacks();
		}
	}

	private static List<IntPtr> effectInterfaceList = null;
	public static bool GetAudioEffectInterfaceList(out List<IntPtr> effect_interface_list)
	{
		if (CriAtomPlugin.IsLibraryInitialized() == true) {
			effect_interface_list = null;
			return false;
		}
		if (effectInterfaceList == null) {
			effectInterfaceList = new List<IntPtr>();
		}
		effect_interface_list = effectInterfaceList;
		return true;
	}

	public static void SetConfigParameters(int max_virtual_voices,
		int max_voice_limit_groups, int max_categories,
		System.Byte max_aisacs, System.Byte max_bus_sends,
		int max_sequence_events_per_frame, int max_beatsync_callbacks_per_frame,
		int max_cuelink_callbacks_per_frame,
		int num_standard_memory_voices, int num_standard_streaming_voices,
		int num_hca_mx_memory_voices, int num_hca_mx_streaming_voices,
		int output_sampling_rate, int num_asr_output_channels, CriAtom.SpeakerMapping speakerMapping,
		bool uses_in_game_preview, float server_frequency,
		int max_parameter_blocks,  int categories_per_playback,
		int max_faders, int num_buses, float max_pitch,
		CriAtomEx.SoundRendererType sound_renderer_type, bool enable_sonicsync_for_common,
		bool enable_atom_sound_disabled_mode)
	{
		criAtomUnity_SetConfigParameters(max_virtual_voices,
			max_voice_limit_groups, max_categories,
			max_aisacs, max_bus_sends,
			max_sequence_events_per_frame, max_beatsync_callbacks_per_frame,
			max_cuelink_callbacks_per_frame,
			num_standard_memory_voices, num_standard_streaming_voices,
			num_hca_mx_memory_voices, num_hca_mx_streaming_voices,
			output_sampling_rate, num_asr_output_channels, speakerMapping,
			uses_in_game_preview, server_frequency,
			max_parameter_blocks, categories_per_playback,
			max_faders, num_buses, max_pitch, sound_renderer_type, enable_sonicsync_for_common,
			enable_atom_sound_disabled_mode
			);

		CriAtomPlugin.isConfigured = true;
	}

	public static void SetConfigMonitorParametes(int max_preview_objects,
		int communication_buffer_size, int playback_position_update_interval)
	{
		Debug.Assert(max_preview_objects >= 0 && communication_buffer_size >= 0 && playback_position_update_interval >= 0);
		criAtomUnity_SetConfigMonitorParameters((uint)max_preview_objects,
			(uint)(communication_buffer_size * 1024), playback_position_update_interval);
	}

	public static void SetConfigAdditionalParameters_EDITOR(bool enable_user_pcm_output, int user_pcm_buffer_length)
	{
		criAtomUnity_SetConfigAdditionalParameters_EDITOR(enable_user_pcm_output);
		CriAtomExAsr.SetPcmBufferSize(user_pcm_buffer_length);
	}

	public static void SetConfigAdditionalParameters_PC(long buffering_time_pc, bool use_microsoft_spatial_sound)
	{
		criAtomUnity_SetConfigAdditionalParameters_PC(buffering_time_pc, use_microsoft_spatial_sound);
	}


	public static void SetConfigAdditionalParameters_IOS(bool enable_sonicsync, uint buffering_time_ios, bool override_ipod_music_ios, bool enable_os_notification_handling)
	{
		criAtomUnity_SetConfigAdditionalParameters_IOS(enable_sonicsync, buffering_time_ios, override_ipod_music_ios, enable_os_notification_handling);
	}

	public static void SetConfigAdditionalParameters_ANDROID(bool enable_sonicsync,
															 int num_low_delay_memory_voices, int num_low_delay_streaming_voices,
															 int sound_buffering_time,        int sound_start_buffering_time,
															 bool use_fast_mixer,             bool use_aaudio,
															 int stream_type)
	{
		if (!enable_sonicsync) {
			stream_type = 0;
		}
		criAtomUnity_SetConfigAdditionalParameters_ANDROID(enable_sonicsync,
														   num_low_delay_memory_voices, num_low_delay_streaming_voices,
														   sound_buffering_time,        sound_start_buffering_time,
														   use_fast_mixer,              stream_type);
#if !UNITY_EDITOR && UNITY_ANDROID
		if (use_fast_mixer == true) {
			IntPtr android_context = IntPtr.Zero;
			using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			using (AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity")) {
				android_context = activity.GetRawObject();
				criAtomUnity_ApplyHardwareProperty_ANDROID(android_context);
			}
		}
		criAtomUnity_EnableAAudio_ANDROID(use_aaudio);
#endif
	}


	public static void SetMaxSamplingRateForStandardVoicePool(int sampling_rate_for_memory, int sampling_rate_for_streaming)
	{
		criAtomUnity_SetMaxSamplingRateForStandardVoicePool(sampling_rate_for_memory, sampling_rate_for_streaming);
	}

	public static int GetRequiredMaxVirtualVoices(CriAtomConfig atomConfig)
	{
		/* バーチャルボイスは、全ボイスプールのボイスの合計値よりも多くなくてはならない */
		int requiredVirtualVoices = 0;

		requiredVirtualVoices += atomConfig.standardVoicePoolConfig.memoryVoices;
		requiredVirtualVoices += atomConfig.standardVoicePoolConfig.streamingVoices;
		requiredVirtualVoices += atomConfig.hcaMxVoicePoolConfig.memoryVoices;
		requiredVirtualVoices += atomConfig.hcaMxVoicePoolConfig.streamingVoices;

		#if UNITY_ANDROID
		requiredVirtualVoices += atomConfig.androidLowLatencyStandardVoicePoolConfig.memoryVoices;
		requiredVirtualVoices += atomConfig.androidLowLatencyStandardVoicePoolConfig.streamingVoices;
		#endif

		return requiredVirtualVoices;
	}

	public static void InitializeLibrary()
	{
		CriWare.Common.CheckBinaryVersionCompatibility();

		/* 初期化カウンタの更新 */
		CriAtomPlugin.initializationCount++;
		if (CriAtomPlugin.initializationCount != 1) {
			return;
		}

		/* シーン実行前に初期化済みの場合は終了させる */
		if (CriAtomPlugin.IsLibraryInitialized() == true) {
			CriAtomPlugin.FinalizeLibrary();
			CriAtomPlugin.initializationCount = 1;
		}

		/* 初期化パラメータが設定済みかどうかを確認 */
		if (CriAtomPlugin.isConfigured == false) {
			Debug.Log("[CRIWARE] Atom initialization parameters are not configured. "
				+ "Initializes Atom by default parameters.");
		}

		/* 依存ライブラリの初期化 */
		CriFsPlugin.InitializeLibrary();

		/* ライブラリの初期化 */
		OnBeforeInitialize?.Invoke();
		CriAtomPlugin.criAtomUnity_Initialize();
		OnInitialized?.Invoke();
		OnInitialized = null;


		/* CriAtomServerのインスタンスを生成 */
		#if UNITY_EDITOR
		/* ゲームプレビュー時のみ生成する */
		if (UnityEngine.Application.isPlaying) {
			CriAtomServer.CreateInstance();
		}
		#else
		CriAtomServer.CreateInstance();
		#endif

		/* CriAtomListenerが存在しない場合のためのダミーリスナーを生成 */
		CriAtomListener.CreateDummyNativeListener();
	}

	public static bool IsLibraryInitialized()
	{
		/* ライブラリが初期化済みかチェック */
		return criAtomUnity_IsInitialized();
	}

	public static void FinalizeLibrary()
	{
		/* 初期化カウンタの更新 */
		CriAtomPlugin.initializationCount--;
		if (CriAtomPlugin.initializationCount < 0) {
			CriAtomPlugin.initializationCount = 0;
			if (CriAtomPlugin.IsLibraryInitialized() == false) {
				return;
			}
		}
		if (CriAtomPlugin.initializationCount != 0) {
			return;
		}

		/* CriAtomListenerが存在しない場合のためのダミーリスナーを破棄 */
		CriAtomListener.DestroyDummyNativeListener();

		/* CriAtomServerのインスタンスを破棄 */
		CriAtomServer.DestroyInstance();

		/* 未破棄のDisposableを破棄 */
		CriDisposableObjectManager.CallOnModuleFinalization(CriDisposableObjectManager.ModuleType.Atom);

		/* ユーザエフェクトインタフェースのリストをクリア */
		if (effectInterfaceList != null) {
			effectInterfaceList.Clear();
			effectInterfaceList = null;
		}

		/* ライブラリの終了 */
		OnBeforeFinalize?.Invoke();
		CriAtomPlugin.criAtomUnity_Finalize();
		OnFinalized?.Invoke();
		OnFinalized = null;

		/* 依存ライブラリの終了 */
		CriFsPlugin.FinalizeLibrary();
	}

#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoadMethod]
	private static void ForceFinalize()
	{
		if(UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode){
			return;
		}
		initializationCount = 1;
		FinalizeLibrary();
	}
#endif

	public static void Pause(bool pause)
	{
		if (isInitialized) {
			criAtomUnity_Pause(pause);
		}
	}

	private static bool isConfigured = false;
	private static float timeSinceStartup = 0;
	private static CriWare.Common.CpuUsage cpuUsage;
	public static CriWare.Common.CpuUsage GetCpuUsage()
	{
		float currentTimeSinceStartup = Time.realtimeSinceStartup;
		if (currentTimeSinceStartup - timeSinceStartup > 1.0f) {
			CriAtomEx.PerformanceInfo info;
			CriAtomEx.GetPerformanceInfo(out info);

			cpuUsage.last = info.lastServerTime * 100.0f / info.averageServerInterval;
			cpuUsage.average = info.averageServerTime * 100.0f / info.averageServerInterval;
			cpuUsage.peak = info.maxServerTime * 100.0f / info.averageServerInterval;

			CriAtomEx.ResetPerformanceMonitor();
			timeSinceStartup = currentTimeSinceStartup;
		}
		return cpuUsage;
	}

	/**
	 * <summary>出力サンプリングレートの取得</summary>
	 * <returns>出力サンプリングレート</returns>
	 * <remarks>
	 * <para header='説明'>Atom ライブラリが出力する PCM データのサンプリングレートを取得します。</para>
	 * </remarks>
	 */
	public static int GetOutputSamplingRate()
	{
		return criAtomUnity_GetAsrOutputSamplingRate();
	}


	/**
	 * <summary>出力チャンネル数の取得</summary>
	 * <returns>出力チャンネル数</returns>
	 * <remarks>
	 * <para header='説明'>Atom ライブラリが出力する PCM データのチャンネル数を取得します。</para>
	 * </remarks>
	 */
	 public static int GetOutputChannels()
	{
		return criAtomUnity_GetAsrOutputChannels();
	}

	public static bool IsInitializedForPcmOutput()
	{
	#if UNITY_EDITOR
		return criAtomUnity_IsInitializedForPcmOutput_EDITOR();
	#else
		return false;
	#endif
	}

	private static int CRIATOMUNITY_PARAMETER_ID_LOOP_COUNT = 0;
	private static ushort CRIATOMPARAMETER2_ID_INVALID = ushort.MaxValue;

	public static ushort GetLoopCountParameterId()
	{
		ushort ret = criAtomUnity_GetNativeParameterId(CRIATOMUNITY_PARAMETER_ID_LOOP_COUNT);
		if (ret == CRIATOMPARAMETER2_ID_INVALID) {
			throw new Exception("GetNativeParameterId failed.");
		}
		return ret;
	}

	/**
	 * <summary>キュー内の波形データの取得</summary>
	 * <returns>取得に成功したか</returns>
	 * <param name='acb'>ACBハンドル</param>
	 * <param name='cueName'>キュー名</param>
	 * <param name='decodeLpcmBuffer'>取得結果を格納する配列</param>
	 * <remarks>
	 * <para header='説明'>指定したキュー内の波形データを取得します。<br/>
	 * キュー内に複数の波形が含まれる場合、取得の対象となるのは最初の1波形のみです。<br/>
	 * <br/>
	 * 取得したデータは[サンプル数*チャンネル数]の長さのインターリーブされた配列となります。<br/>
	 * <paramref name='decodeLpcmBuffer'/>には十分な長さの配列を指定してください。<br/><code>
	 * acbHandle.GetWaveFormInfo(cueName, out var waveformInfo);
	 * var resultArray = new short[waveformInfo.numChannels * waveformInfo.numSamples];
	 * CriAtomPlugin.GetWaveSamples(acbHandle, cueName, resultArray);
	 * </code>
	 * </para>
	 * </remarks>
	 */
	public static bool GetWaveSamples(CriAtomExAcb acb, string cueName, System.Int16[] decodeLpcmBuffer)
	{
#if !UNITY_STANDALONE && !UNITY_EDITOR
		throw new NotSupportedException("[CRIWARE] This platform does not support the method CriAtomPlugin.GetWaveSamples");
#else
		var handle = GCHandle.Alloc(decodeLpcmBuffer, GCHandleType.Pinned);
		var result = criAtomUnityThumbnail_LoadWaveform(acb.nativeHandle, cueName, handle.AddrOfPinnedObject(), decodeLpcmBuffer.Length);
		handle.Free();
		return result;
#endif
	}




	#region Private Methods
	#endregion

	#region DLL Import
#if !CRIWARE_ENABLE_HEADLESS_MODE && (UNITY_STANDALONE || UNITY_EDITOR)
	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	private static extern bool criAtomUnityThumbnail_LoadWaveform(IntPtr acbHn, string cue_name, IntPtr decode_lpcm_buffer, System.Int64 decodeLpcmBufferLength);
#else
	private static bool criAtomUnityThumbnail_LoadWaveform(IntPtr acbHn, string cue_name, IntPtr decode_lpcm_buffer, System.Int64 decodeLpcmBufferLength) {return false;}
#endif
	#if !CRIWARE_ENABLE_HEADLESS_MODE
	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	private static extern void criAtomUnity_SetConfigParameters(int max_virtual_voices,
		int max_voice_limit_groups, int max_categories,
		System.Byte max_aisacs, System.Byte max_bus_sends,
		int max_sequence_events_per_frame, int max_beatsync_callbacks_per_frame,
		int max_cuelink_callbacks_per_frame,
		int num_standard_memory_voices, int num_standard_streaming_voices,
		int num_hca_mx_memory_voices, int num_hca_mx_streaming_voices,
		int output_sampling_rate, int num_asr_output_channels, CriAtom.SpeakerMapping speakerMapping,
		bool uses_in_game_preview, float server_frequency,
		int max_parameter_blocks, int categories_per_playback,
		int max_faders, int num_buses, float max_pitch,
		CriAtomEx.SoundRendererType sound_renderer_type, bool enable_sonicsync_for_common,
		bool enable_atom_sound_disabled_mode);

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	private static extern void criAtomUnity_SetConfigMonitorParameters(uint max_preivew_objects,
		uint communication_buffer_size, int playback_position_update_interval);

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	private static extern void criAtomUnity_SetConfigAdditionalParameters_EDITOR(bool enable_user_pcm_out_mode);

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	private static extern void criAtomUnity_SetConfigAdditionalParameters_PC(long buffering_time_pc, bool use_microsoft_spatial_sound);


	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	private static extern void criAtomUnity_SetConfigAdditionalParameters_IOS(bool enable_sonicsync, uint buffering_time_ios, bool override_ipod_music_ios, bool enable_os_notification_handling);

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	private static extern void criAtomUnity_SetConfigAdditionalParameters_ANDROID(bool enable_sonicsync, 
																				  int num_low_delay_memory_voices, int num_low_delay_streaming_voices,
																				  int sound_buffering_time,        int sound_start_buffering_time,
																				  bool apply_hw_property,          int stream_type);

	#if !UNITY_EDITOR && UNITY_ANDROID
	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	private static extern void criAtomUnity_ApplyHardwareProperty_ANDROID(IntPtr android_context);

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	private static extern void criAtomUnity_EnableAAudio_ANDROID(bool flag);
	#endif


	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	private static extern void criAtomUnity_Initialize();

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	public static extern bool criAtomUnity_IsInitialized();

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	private static extern void criAtomUnity_Finalize();

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	private static extern void criAtomUnity_Pause(bool pause);

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	public static extern uint criAtomUnity_GetAllocatedHeapSize();

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	public static extern void criAtomUnity_ControlDataCompatibility(int code);

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	public static extern void criAtomUnitySequencer_SetEventCallback(IntPtr cbfunc, string separator_string);

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	public static extern void criAtomUnitySequencer_SetCallback(IntPtr cbfunc);

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	private static extern void criAtomUnitySequencer_ExecuteQueuedEventCallbacks();

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	public static extern void criAtomUnity_SetBeatSyncCallback(IntPtr cbfunc);

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	private static extern void criAtomUnity_ExecuteQueuedBeatSyncCallbacks();

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	public static extern void criAtomUnity_SetCueLinkCallback(IntPtr cbfunc);

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	private static extern void criAtomUnity_ExecuteQueuedCueLinkCallbacks();

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	private static extern void criAtomUnity_SetMaxSamplingRateForStandardVoicePool(int sampling_rate_for_memory, int sampling_rate_for_streaming);

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	public static extern void criAtomUnity_BeginLeCompatibleMode();

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	public static extern void criAtomUnity_EndLeCompatibleMode();


	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	public static extern ushort criAtomUnity_GetNativeParameterId(int id);

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	private static extern bool criAtomUnity_IsInitializedForPcmOutput_EDITOR();

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	private static extern int criAtomUnity_GetAsrOutputSamplingRate();

	[DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
	private static extern int criAtomUnity_GetAsrOutputChannels();

	#else
	private static void criAtomUnity_SetConfigParameters(int max_virtual_voices,
		int max_voice_limit_groups, int max_categories,
		System.Byte max_aisacs, System.Byte max_bus_sends,
		int max_sequence_events_per_frame, int max_beatsync_callbacks_per_frame,
		int max_cuelink_callbacks_per_frame,
		int num_standard_memory_voices, int num_standard_streaming_voices,
		int num_hca_mx_memory_voices, int num_hca_mx_streaming_voices,
		int output_sampling_rate, int num_asr_output_channels, CriAtom.SpeakerMapping speakerMapping,
		bool uses_in_game_preview, float server_frequency,
		int max_parameter_blocks, int categories_per_playback,
		int max_faders, int num_buses, float max_pitch,
		CriAtomEx.SoundRendererType sound_renderer_type, bool enable_sonicsync_for_common,
		bool enable_atom_sound_disabled_mode) { }
	private static void criAtomUnity_SetConfigMonitorParameters(uint max_preivew_objects,
		uint communication_buffer_size, int playback_position_update_interval) { }
	private static void criAtomUnity_SetConfigAdditionalParameters_EDITOR(bool enable_user_pcm_out_mode) { }
	private static void criAtomUnity_SetConfigAdditionalParameters_PC(long buffering_time_pc, bool use_microsoft_spatial_sound) { }
	private static void criAtomUnity_SetConfigAdditionalParameters_IOS(bool enable_sonicsync, uint buffering_time_ios, bool override_ipod_music_ios, bool enable_os_notification_handling) { }
	private static void criAtomUnity_SetConfigAdditionalParameters_ANDROID(bool enable_sonicsync, 
																		   int num_low_delay_memory_voices, int num_low_delay_streaming_voices,
																		   int sound_buffering_time,        int sound_start_buffering_time,
																		   bool apply_hw_property,          int stream_type) { }
	#if !UNITY_EDITOR && UNITY_ANDROID
	private static void criAtomUnity_ApplyHardwareProperty_ANDROID(IntPtr android_context) { }
	private static void criAtomUnity_EnableAAudio_ANDROID(bool flag) { }
	#endif

	private static bool _dummyInitialized = false;
	private static void criAtomUnity_Initialize() { _dummyInitialized = true; }
	public static bool criAtomUnity_IsInitialized() { return _dummyInitialized; }
	private static void criAtomUnity_Finalize() { _dummyInitialized = false; }
	private static void criAtomUnity_Pause(bool pause) { }
	public static uint criAtomUnity_GetAllocatedHeapSize() { return 0; }
	public static void criAtomUnity_ControlDataCompatibility(int code) { }
	public static void criAtomUnitySequencer_SetEventCallback(IntPtr cbfunc, string separator_string) { }
	public static void criAtomUnitySequencer_SetCallback(IntPtr cbfunc) { }
	public static void criAtomUnitySequencer_ExecuteQueuedEventCallbacks() { }
	public static void criAtomUnity_SetBeatSyncCallback(IntPtr cbfunc) { }
	public static void criAtomUnity_ExecuteQueuedBeatSyncCallbacks() { }
	public static void criAtomUnity_SetCueLinkCallback(IntPtr cbfunc) { }
	public static void criAtomUnity_ExecuteQueuedCueLinkCallbacks() { }
	private static void criAtomUnity_SetMaxSamplingRateForStandardVoicePool(int sampling_rate_for_memory, int sampling_rate_for_streaming) { }
	public static void criAtomUnity_BeginLeCompatibleMode() { }
	public static void criAtomUnity_EndLeCompatibleMode() { }
	public static ushort criAtomUnity_GetNativeParameterId(int id) { return 0; }
	private static bool criAtomUnity_IsInitializedForPcmOutput_EDITOR() { return false; }
	private static int criAtomUnity_GetAsrOutputSamplingRate() { return 0; }
	private static int criAtomUnity_GetAsrOutputChannels() { return 0; }
	#endif
	#endregion

	#endregion
}

[Serializable]
public class CriAtomCueSheet
{
	public string name = "";
	public string acbFile = "";
	public string awbFile = "";
	public CriAtomExAcb acb;
	public CriAtomExAcbLoader.Status loaderStatus = CriAtomExAcbLoader.Status.Stop;
	public bool IsLoading { get { return loaderStatus == CriAtomExAcbLoader.Status.Loading; } }
	public bool IsError { get { return (loaderStatus == CriAtomExAcbLoader.Status.Error) || (!IsLoading && acb == null); } }
}

} //namespace CriWare

/**
 * @}
 */

/**
 * \addtogroup CRIATOM_UNITY_COMPONENT
 * @{
 */

namespace CriWare {

/**
 * <summary>サウンド再生全体を制御するためのコンポーネントです。</summary>
 * <remarks>
 * <para header='説明'>各シーンにひとつ用意しておく必要があります。<br/>
 * UnityEditor上でCRI Atomウィンドウを使用して CriAtomSource を作成した場合、
 * 「CRIWARE」という名前を持つオブジェクトとして自動的に作成されます。通常はユーザーが作成する必要はありません。</para>
 * </remarks>
 */
[AddComponentMenu("CRIWARE/CRI Atom")]
public partial class CriAtom : CriMonoBehaviour
{
		#region Types

		/**
		 * <summary>ミキサーのスピーカーマッピング種別</summary>
		 * <remarks>
		 * <para header='説明'>ASRのスピーカーマッピングを指定します。<br/></para>
		 * </remarks>
		 * <seealso cref='CriAtomExAsrRack.Config.speakerMapping'/>
		 * <seealso cref='CriAtomConfig.speakerMapping'/>
		 */
		public enum SpeakerMapping : System.Int32
		{
			Auto = 0,    /**< 自動設定 */
			Monaural,    /**< 1ch */
			Stereo,      /**< 2ch */
			Ch5_1,       /**< 5.1ch */
			Ch7_1,       /**< 7.1ch */
			Ch5_1_2,     /**< 5.1.2ch */
			Ch7_1_2,     /**< 7.1.2ch */
			Ch7_1_4,     /**< 7.1.4ch */
			Ch7_1_4_4,   /**< 7.1.4.4ch */
			Ambisonics1p,/**< 1st order Ambisonics */
			Ambisonics2p,/**< 2st order Ambisonics */
			Ambisonics3p,/**< 3st order Ambisonics */
			Object,      /**< Object Base Audio */
			Custom,      /**< Custom Speaker Mapping */
		}

		#endregion

	/* @cond DOXYGEN_IGNORE */
	public string acfFile = "";
	private bool acfIsLoading = false;
#if CRIWARE_FORCE_LOAD_ASYNC
	private byte[] acfData = null;
#endif
	public CriAtomCueSheet[] cueSheets = {};
	public string dspBusSetting = "";
	public bool dontDestroyOnLoad = false;

	private static CriAtomExSequencer.EventCallback eventUserCallback = null;
	internal static bool HasUserCallback => eventUserCallback != null;
	internal static event CriAtomExSequencer.EventCallback OnEventSequencerCallback {
		add {
			RegisterEventCallbackChain(value);
		}
		remove {
			UnregisterEventCallbackChain(value);
		}
	}
	private static CriAtomExSequencer.EventCbFunc eventUserCbFunc = null;

	private static event CriAtomExBeatSync.CbFunc beatsyncUserCbFunc = null;
	internal static bool HasBeatSyncCallback => beatsyncUserCbFunc != null;
	internal static event CriAtomExBeatSync.CbFunc OnBeatSyncCallback {
		add {
			RegisterBeatSyncCallbackChain(value);
		}
		remove {
			UnregisterBeatSyncCallbackChain(value);
		}
	}
	private static CriAtomExBeatSync.CbFunc obsoleteBeatSyncFunc = null;

	private static event CriAtomEx.CueLinkCbFunc cueLinkUserCbFunc = null;
	internal static bool HasCueLinkCallback => cueLinkUserCbFunc != null;
	internal static event CriAtomEx.CueLinkCbFunc OnCueLinkCallback {
		add {
			RegisterCueLinkCallbackChain(value);
		}
		remove {
			UnregisterCueLinkCallbackChain(value);
		}
	}

	private static CriAtom instance {
		get; set;
	}
	private GCHandle acfRegisterGCHandle;

	/* @endcond */

	#region Functions

	/**
	 * <summary>DSPバス設定のアタッチ</summary>
	 * <param name='settingName'>DSPバス設定の名前</param>
	 * <remarks>
	 * <para header='説明'>DSPバス設定からDSPバスを構築してサウンドレンダラにアタッチします。<br/>
	 * 現在設定中のDSPバス設定を切り替える場合は、一度デタッチしてから再アタッチしてください。
	 * <br/></para>
	 * <para header='注意'>本関数は完了復帰型の関数です。<br/>
	 * 本関数を実行すると、しばらくの間Atomライブラリのサーバ処理がブロックされます。<br/>
	 * 音声再生中に本関数を実行すると、音途切れ等の不具合が発生する可能性があるため、
	 * 本関数の呼び出しはシーンの切り替わり等、負荷変動を許容できるタイミングで行ってください。<br/></para>
	 * </remarks>
	 * <seealso cref='CriAtom::DetachDspBusSetting'/>
	 */
	public static void AttachDspBusSetting(string settingName)
	{
		CriAtom.instance.dspBusSetting = settingName;
		if (!String.IsNullOrEmpty(settingName)) {
			CriAtomEx.AttachDspBusSetting(settingName);
		} else {
			CriAtomEx.DetachDspBusSetting();
		}
	}

	/**
	 * <summary>DSPバス設定のデタッチ</summary>
	 * <remarks>
	 * <para header='説明'>DSPバス設定をデタッチします。<br/>
	 * <br/></para>
	 * <para header='注意'>本関数は完了復帰型の関数です。<br/>
	 * 本関数を実行すると、しばらくの間Atomライブラリのサーバ処理がブロックされます。<br/>
	 * 音声再生中に本関数を実行すると、音途切れ等の不具合が発生する可能性があるため、
	 * 本関数の呼び出しはシーンの切り替わり等、負荷変動を許容できるタイミングで行ってください。<br/></para>
	 * </remarks>
	 * <seealso cref='CriAtom::DetachDspBusSetting'/>
	 */
	public static void DetachDspBusSetting()
	{
		CriAtom.instance.dspBusSetting = "";
		CriAtomEx.DetachDspBusSetting();
	}

	/**
	 * <summary>キューシートの取得</summary>
	 * <param name='name'>キューシート名</param>
	 * <returns>キューシートオブジェクト</returns>
	 * <remarks>
	 * <para header='説明'>引数に指定したキューシート名を元に登録済みのキューシートオブジェクトを取得します。<br/></para>
	 * </remarks>
	 */
	public static CriAtomCueSheet GetCueSheet(string name)
	{
		return CriAtom.instance.GetCueSheetInternal(name);
	}

	/**
	 * <summary>キューシートの追加</summary>
	 * <param name='name'>キューシート名</param>
	 * <param name='acbFile'>ACBファイルパス</param>
	 * <param name='awbFile'>AWBファイルパス</param>
	 * <param name='binder'>バインダオブジェクト(オプション)</param>
	 * <returns>キューシートオブジェクト</returns>
	 * <remarks>
	 * <para header='説明'>引数に指定したファイルパス情報を元にキューシートの追加を行います。<br/>
	 * 同時に複数のキューシートを登録することが可能です。<br/>
	 * <br/>
	 * 各ファイルパスに対して相対パスを指定した場合は StreamingAssets フォルダからの相対でファイルをロードします。<br/>
	 * 絶対パス、あるいはURLを指定した場合には指定したパスでファイルをロードします。<br/>
	 * <br/>
	 * CPKファイルにパッキングされたACBファイルとAWBファイルからキューシートを追加する場合は、
	 * binder引数にCPKをバインドしたバインダを指定してください。<br/>
	 * なお、バインダ機能はADXLEではご利用になれません。<br/></para>
	 * </remarks>
	 */
	public static CriAtomCueSheet AddCueSheet(string name, string acbFile, string awbFile, CriFsBinder binder = null)
	{
	#if CRIWARE_FORCE_LOAD_ASYNC
		return CriAtom.AddCueSheetAsync(name, acbFile, awbFile, binder);
	#else
		CriAtomCueSheet cueSheet = CriAtom.instance.AddCueSheetInternal(name, acbFile, awbFile, binder);
		if (Application.isPlaying) {
			cueSheet.acb = CriAtom.instance.LoadAcbFile(binder, acbFile, awbFile);
		}
		return cueSheet;
	#endif
	}

	/**
	 * <summary>非同期でのキューシートの追加</summary>
	 * <param name='name'>キューシート名</param>
	 * <param name='acbFile'>ACBファイルパス</param>
	 * <param name='awbFile'>AWBファイルパス</param>
	 * <param name='binder'>バインダオブジェクト(オプション)</param>
	 * <param name='loadAwbOnMemory'>AWBファイルをメモリ上にロードするか(オプション)</param>
	 * <returns>キューシートオブジェクト</returns>
	 * <remarks>
	 * <para header='説明'>引数に指定したファイルパス情報を元に、非同期でキューシートの追加を行います。<br/>
	 * 同時に複数のキューシートを登録することが可能です。<br/>
	 * <br/>
	 * 各ファイルパスに対して相対パスを指定した場合は StreamingAssets フォルダからの相対でファイルをロードします。<br/>
	 * 絶対パス、あるいはURLを指定した場合には指定したパスでファイルをロードします。<br/>
	 * <br/>
	 * CPKファイルにパッキングされたACBファイルとAWBファイルからキューシートを追加する場合は、
	 * binder引数にCPKをバインドしたバインダを指定してください。<br/>
	 * なお、バインダ機能はADXLEではご利用になれません。<br/>
	 * <br/>
	 * 戻り値のキューシートオブジェクトの CriAtomCueSheet::isLoading メンバが true を返す間はロード中です。<br/>
	 * 必ず false を返すのを確認してからキューの再生等を行うようにしてください。<br/>
	 * <br/>
	 * loadAwbOnMemory が false の場合、AWBファイルのヘッダ部分のみをメモリ上にロードしストリーム再生を行います。<br/>
	 * loadAwbOnMemory を true に設定すると、AWBファイル全体をメモリ上にロードするため実質メモリ再生になります。<br/>
	 * WebGL(Editor実行時)では内部都合上、 loadAwbOnMemory が強制的にtrueになります。<br/></para>
	 * </remarks>
	 */
	public static CriAtomCueSheet AddCueSheetAsync(string name, string acbFile, string awbFile, CriFsBinder binder = null, bool loadAwbOnMemory = false)
	{
	#if UNITY_EDITOR && UNITY_WEBGL
		loadAwbOnMemory = true;
	#endif
		CriAtomCueSheet cueSheet = CriAtom.instance.AddCueSheetInternal(name, acbFile, awbFile, binder);
		if (Application.isPlaying) {
			CriAtom.instance.LoadAcbFileAsync(cueSheet, binder, acbFile, awbFile, loadAwbOnMemory);
		}
		return cueSheet;
	}

	/**
	 * <summary>キューシートの追加（メモリからの読み込み）</summary>
	 * <param name='name'>キューシート名</param>
	 * <param name='acbData'>ACBデータ</param>
	 * <param name='awbFile'>AWBファイルパス</param>
	 * <param name='awbBinder'>AWB用バインダオブジェクト(オプション)</param>
	 * <returns>キューシートオブジェクト</returns>
	 * <remarks>
	 * <para header='説明'>引数に指定したデータとファイルパス情報からキューシートの追加を行います。<br/>
	 * 同時に複数のキューシートを登録することが可能です。<br/>
	 * <br/>
	 * ファイルパスに対して相対パスを指定した場合は StreamingAssets フォルダからの相対でファイルをロードします。<br/>
	 * 絶対パス、あるいはURLを指定した場合には指定したパスでファイルをロードします。<br/>
	 * <br/>
	 * CPKファイルにパッキングされたAWBファイルからキューシートを追加する場合は、
	 * awbBinder引数にCPKをバインドしたバインダを指定してください。<br/>
	 * なお、バインダ機能はADXLEではご利用になれません。<br/></para>
	 * </remarks>
	 */
	public static CriAtomCueSheet AddCueSheet(string name, byte[] acbData, string awbFile, CriFsBinder awbBinder = null)
	{
	#if CRIWARE_FORCE_LOAD_ASYNC
		return CriAtom.AddCueSheetAsync(name, acbData, awbFile, awbBinder);
	#else
		CriAtomCueSheet cueSheet = CriAtom.instance.AddCueSheetInternal(name, "", awbFile, awbBinder);
		if (Application.isPlaying) {
			cueSheet.acb = CriAtom.instance.LoadAcbData(acbData, awbBinder, awbFile);
		}
		return cueSheet;
	#endif
	}

	/**
	 * <summary>非同期でのキューシートの追加（メモリからの読み込み）</summary>
	 * <param name='name'>キューシート名</param>
	 * <param name='acbData'>ACBデータ</param>
	 * <param name='awbFile'>AWBファイルパス</param>
	 * <param name='awbBinder'>AWB用バインダオブジェクト(オプション)</param>
	 * <param name='loadAwbOnMemory'>AWBファイルをメモリ上にロードするか(オプション)</param>
	 * <returns>キューシートオブジェクト</returns>
	 * <remarks>
	 * <para header='説明'>引数に指定したデータとファイルパス情報からキューシートの追加を行います。<br/>
	 * 同時に複数のキューシートを登録することが可能です。<br/>
	 * <br/>
	 * ファイルパスに対して相対パスを指定した場合は StreamingAssets フォルダからの相対でファイルをロードします。<br/>
	 * 絶対パス、あるいはURLを指定した場合には指定したパスでファイルをロードします。<br/>
	 * <br/>
	 * CPKファイルにパッキングされたAWBファイルからキューシートを追加する場合は、
	 * awbBinder引数にCPKをバインドしたバインダを指定してください。<br/>
	 * なお、バインダ機能はADXLEではご利用になれません。<br/>
	 * <br/>
	 * 戻り値のキューシートオブジェクトの CriAtomCueSheet::isLoading メンバが true を返す間はロード中です。<br/>
	 * 必ず false を返すのを確認してからキューの再生等を行うようにしてください。<br/>
	 * <br/>
	 * loadAwbOnMemory が false の場合、AWBファイルのヘッダ部分のみをメモリ上にロードしストリーム再生を行います。<br/>
	 * loadAwbOnMemory を true に設定すると、AWBファイル全体をメモリ上にロードするため実質メモリ再生になります。<br/>
	 * WebGL(Editor実行時)では内部都合上、 loadAwbOnMemory が強制的にtrueになります。<br/></para>
	 * </remarks>
	 */
	public static CriAtomCueSheet AddCueSheetAsync(string name, byte[] acbData, string awbFile, CriFsBinder awbBinder = null, bool loadAwbOnMemory = false)
	{
	#if UNITY_EDITOR && UNITY_WEBGL
		loadAwbOnMemory = true;
	#endif
		CriAtomCueSheet cueSheet = CriAtom.instance.AddCueSheetInternal(name, "", awbFile, awbBinder);
		if (Application.isPlaying) {
			CriAtom.instance.LoadAcbDataAsync(cueSheet, acbData, awbBinder, awbFile, loadAwbOnMemory);
		}
		return cueSheet;
	}

	/**
	 * <summary>キューシートの削除</summary>
	 * <param name='name'>キューシート名</param>
	 * <remarks>
	 * <para header='説明'>追加済みのキューシートを削除します。<br/></para>
	 * </remarks>
	 */
	public static void RemoveCueSheet(string name)
	{
		if (CriAtom.instance == null) {
			return;
		}
		CriAtom.instance.RemoveCueSheetInternal(name);
	}

	/**
	 * <summary>キューシートのロード完了チェック</summary>
	 * <remarks>
	 * <para header='説明'>全てのキューシートのロード完了をチェックします。<br/></para>
	 * </remarks>
	 */
	public static bool CueSheetsAreLoading {
		get {
			if (CriAtom.instance == null) {
				return false;
			}
			foreach (var cueSheet in CriAtom.instance.cueSheets) {
				if (cueSheet.IsLoading) {
					return true;
				}
			}
			return false;
		}
	}

	/**
	 * <summary>ACBオブジェクトの取得</summary>
	 * <param name='cueSheetName'>キューシート名</param>
	 * <returns>ACBオブジェクト</returns>
	 * <remarks>
	 * <para header='説明'>指定したキューシートに対応するACBオブジェクトを取得します。<br/></para>
	 * </remarks>
	 */
	public static CriAtomExAcb GetAcb(string cueSheetName)
	{
		foreach (var cueSheet in CriAtom.instance.cueSheets) {
			if (cueSheetName == cueSheet.name) {
				return cueSheet.acb;
			}
		}
		Debug.LogWarning("[CRIWARE] Cue sheet named \""+ cueSheetName + "\" is not loaded.");
		return null;
	}

	/**
	 * <summary>カテゴリ名指定でカテゴリボリュームを設定します。</summary>
	 * <param name='name'>カテゴリ名</param>
	 * <param name='volume'>ボリューム</param>
	 */
	public static void SetCategoryVolume(string name, float volume)
	{
		CriAtomExCategory.SetVolume(name, volume);
	}

	/**
	 * <summary>カテゴリID指定でカテゴリボリュームを設定します。</summary>
	 * <param name='id'>カテゴリID</param>
	 * <param name='volume'>ボリューム</param>
	 */
	public static void SetCategoryVolume(int id, float volume)
	{
		CriAtomExCategory.SetVolume(id, volume);
	}

	/**
	 * <summary>カテゴリ名指定でカテゴリボリュームを取得します。</summary>
	 * <param name='name'>カテゴリ名</param>
	 * <returns>ボリューム</returns>
	 */
	public static float GetCategoryVolume(string name)
	{
		return CriAtomExCategory.GetVolume(name);
	}
	/**
	 * <summary>カテゴリID指定でカテゴリボリュームを取得します。</summary>
	 * <param name='id'>カテゴリID</param>
	 * <returns>ボリューム</returns>
	 */
	public static float GetCategoryVolume(int id)
	{
		return CriAtomExCategory.GetVolume(id);
	}

	/**
	 * <summary>バス情報取得を有効にします。</summary>
	 * <param name='busName'>DSPバス名</param>
	 * <param name='sw'>True: 取得を有効にする。 False: 取得を無効にする</param>
	 */
	public static void SetBusAnalyzer(string busName, bool sw)
	{
	#if !UNITY_EDITOR && UNITY_PSP2
		// unsupported
	#else
		if (sw) {
			CriAtomExAsr.AttachBusAnalyzer(busName, 50, 1000);
		} else {
			CriAtomExAsr.DetachBusAnalyzer(busName);
		}
	#endif
	}

	/**
	 * <summary>全てのバス情報取得を有効にします。</summary>
	 * <param name='sw'>True: 取得を有効にする。 False: 取得を無効にする</param>
	 */
	public static void SetBusAnalyzer(bool sw)
	{
	#if !UNITY_EDITOR && UNITY_PSP2
		// unsupported
	#else
		if (sw) {
			CriAtomExAsr.AttachBusAnalyzer(50, 1000);
		} else {
			CriAtomExAsr.DetachBusAnalyzer();
		}
	#endif
	}

	/**
	 * <summary>バス情報を取得します。</summary>
	 * <param name='busName'>DSPバス名</param>
	 * <returns>DSPバス情報</returns>
	 */
	public static CriAtomExAsr.BusAnalyzerInfo GetBusAnalyzerInfo(string busName)
	{
		CriAtomExAsr.BusAnalyzerInfo info;
	#if !UNITY_EDITOR && UNITY_PSP2
		info = new CriAtomExAsr.BusAnalyzerInfo(null);
	#else
		CriAtomExAsr.GetBusAnalyzerInfo(busName, out info);
	#endif
		return info;
	}

	/**
	 * \deprecated
	 * 削除予定の非推奨APIです。
	 * CriAtom.GetBusAnalyzerInfo(string busName)の使用を検討してください。
	*/
	[System.Obsolete("Use CriAtom.GetBusAnalyzerInfo(string busName)")]
	public static CriAtomExAsr.BusAnalyzerInfo GetBusAnalyzerInfo(int busId)
	{
		CriAtomExAsr.BusAnalyzerInfo info;
	#if !UNITY_EDITOR && UNITY_PSP2
		info = new CriAtomExAsr.BusAnalyzerInfo(null);
	#else
		CriAtomExAsr.GetBusAnalyzerInfo(busId, out info);
	#endif
		return info;
	}

	#endregion // Functions

	/* @cond DOXYGEN_IGNORE */
	#region Functions for internal use

	public void Setup()
	{
		if (CriAtom.instance != null && CriAtom.instance != this) {
			var obj = CriAtom.instance.gameObject;
			CriAtom.instance.Shutdown();
			CriAtomEx.UnregisterAcf();
			GameObject.Destroy(obj);
		}

		CriAtom.instance = this;

		CriAtomPlugin.InitializeLibrary();

		if (!String.IsNullOrEmpty(this.acfFile)) {
			string acfPath = Path.Combine(CriWare.Common.streamingAssetsPath, this.acfFile);
			CriAtomEx.RegisterAcf(null, acfPath);
		}

		if (!String.IsNullOrEmpty(dspBusSetting)) {
			AttachDspBusSetting(dspBusSetting);
		}

		foreach (var cueSheet in this.cueSheets) {
			cueSheet.acb = this.LoadAcbFile(null, cueSheet.acbFile, cueSheet.awbFile);
		}

		if (this.dontDestroyOnLoad) {
			GameObject.DontDestroyOnLoad(this.gameObject);
		}
	}

	public void Shutdown()
	{
		foreach (var cueSheet in this.cueSheets) {
			if (cueSheet.acb != null) {
				cueSheet.acb.Dispose();
				cueSheet.acb = null;
			}
		}
		CriAtomPlugin.FinalizeLibrary();
		if (CriAtom.instance == this) {
			CriAtom.instance = null;
		}
	}

	void Awake()
	{
		if (CriAtom.instance != null && CriAtom.instance != this) {
			if (CriAtom.instance.acfFile != this.acfFile) {
				var obj = CriAtom.instance.gameObject;
				CriAtom.instance.Shutdown();
				CriAtomEx.UnregisterAcf();
				GameObject.Destroy(obj);
				return;
			}
			if (CriAtom.instance.dspBusSetting != this.dspBusSetting) {
				CriAtom.AttachDspBusSetting(this.dspBusSetting);
			}
			CriAtom.instance.MargeCueSheet(this.cueSheets, this.dontRemoveExistsCueSheet);
			GameObject.Destroy(this.gameObject);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
	#if UNITY_EDITOR
		if (CriAtomPlugin.previewCallback != null) {
			CriAtomPlugin.previewCallback();
		}
	#endif
		if (CriAtom.instance != null) {
			return;
		}

	#if CRIWARE_FORCE_LOAD_ASYNC
		this.SetupAsync();
	#else
		this.Setup();
	#endif
	}

	void OnDestroy()
	{
		if (this != CriAtom.instance) {
			return;
		}
		if (beatsyncUserCbFunc != null) {
			beatsyncUserCbFunc = null;
			CriAtomPlugin.criAtomUnity_SetBeatSyncCallback(IntPtr.Zero);
		}
		if (this.acfRegisterGCHandle.IsAllocated) {
			CriAtomEx.UnregisterAcf();
			this.acfRegisterGCHandle.Free();
		}
		this.Shutdown();
	}

	public override void CriInternalUpdate() { }

	public override void CriInternalLateUpdate() { }

	public CriAtomCueSheet GetCueSheetInternal(string name)
	{
		for (int i = 0; i < this.cueSheets.Length; i++) {
			CriAtomCueSheet cueSheet = this.cueSheets[i];
			if (cueSheet.name == name) {
				return cueSheet;
			}
		}
		return null;
	}

	public CriAtomCueSheet AddCueSheetInternal(string name, string acbFile, string awbFile, CriFsBinder binder)
	{
		var cueSheets = new CriAtomCueSheet[this.cueSheets.Length + 1];
		this.cueSheets.CopyTo(cueSheets, 0);
		this.cueSheets = cueSheets;

		var cueSheet = new CriAtomCueSheet();
		this.cueSheets[this.cueSheets.Length - 1] = cueSheet;
		if (String.IsNullOrEmpty(name)) {
			cueSheet.name = Path.GetFileNameWithoutExtension(acbFile);
		} else {
			cueSheet.name = name;
		}
		cueSheet.acbFile = acbFile;
		cueSheet.awbFile = awbFile;
		return cueSheet;
	}

	public void RemoveCueSheetInternal(string name)
	{
		int index = -1;
		for (int i = 0; i < this.cueSheets.Length; i++) {
			if (name == this.cueSheets[i].name) {
				index = i;
			}
		}
		if (index < 0) {
			return;
		}

		var cueSheet = this.cueSheets[index];
		if (cueSheet.acb != null) {
			cueSheet.acb.Dispose();
			cueSheet.acb = null;
		}

		var cueSheets = new CriAtomCueSheet[this.cueSheets.Length - 1];
		Array.Copy(this.cueSheets, 0, cueSheets, 0, index);
		Array.Copy(this.cueSheets, index + 1, cueSheets, index, this.cueSheets.Length - index - 1);
		this.cueSheets = cueSheets;
	}

	public bool dontRemoveExistsCueSheet = false;

	/*
	 * newDontRemoveExistsCueSheet == false の場合、古いキューシートを登録解除して、新しいキューシートの登録を行う。
	 * ただし同じキューシートの再登録は回避する
	 */
	private void MargeCueSheet(CriAtomCueSheet[] newCueSheets, bool newDontRemoveExistsCueSheet)
	{
		if (!newDontRemoveExistsCueSheet) {
			for (int i = 0; i < this.cueSheets.Length; ) {
				int index = Array.FindIndex(newCueSheets, sheet => sheet.name == this.cueSheets[i].name);
				if (index < 0) {
					CriAtom.RemoveCueSheet(this.cueSheets[i].name);
				} else {
					i++;
				}
			}
		}

		foreach (var sheet1 in newCueSheets) {
			var sheet2 = this.GetCueSheetInternal(sheet1.name);
			if (sheet2 == null) {
				CriAtom.AddCueSheet(null, sheet1.acbFile, sheet1.awbFile, null);
			}
		}
	}

	private CriAtomExAcb LoadAcbFile(CriFsBinder binder, string acbFile, string awbFile)
	{
		if (String.IsNullOrEmpty(acbFile)) {
			return null;
		}

		string acbPath = acbFile;
		if ((binder == null) && CriWare.Common.IsStreamingAssetsPath(acbPath)) {
			acbPath = Path.Combine(CriWare.Common.streamingAssetsPath, acbPath);
		}

		string awbPath = awbFile;
		if (!String.IsNullOrEmpty(awbPath)) {
			if ((binder == null) && CriWare.Common.IsStreamingAssetsPath(awbPath)) {
				awbPath = Path.Combine(CriWare.Common.streamingAssetsPath, awbPath);
			}
		}

		return CriAtomExAcb.LoadAcbFile(binder, acbPath, awbPath);
	}

	private CriAtomExAcb LoadAcbData(byte[] acbData, CriFsBinder binder, string awbFile)
	{
		if (acbData == null) {
			return null;
		}

		string awbPath = awbFile;
		if (!String.IsNullOrEmpty(awbPath)) {
			if ((binder == null) && CriWare.Common.IsStreamingAssetsPath(awbPath)) {
				awbPath = Path.Combine(CriWare.Common.streamingAssetsPath, awbPath);
			}
		}

		return CriAtomExAcb.LoadAcbData(acbData, binder, awbPath);
	}

#if CRIWARE_FORCE_LOAD_ASYNC
	private void SetupAsync()
	{
		CriAtom.instance = this;

		CriAtomPlugin.InitializeLibrary();

		if (this.dontDestroyOnLoad) {
			GameObject.DontDestroyOnLoad(this.gameObject);
		}

		if (!String.IsNullOrEmpty(this.acfFile)) {
			this.acfIsLoading = true;
			StartCoroutine(RegisterAcfAsync(this.acfFile, this.dspBusSetting));
		}

		foreach (var cueSheet in this.cueSheets) {
			#if UNITY_EDITOR
			bool loadAwbOnMemory = true;
			#else
			bool loadAwbOnMemory = false;
			#endif
			StartCoroutine(LoadAcbFileCoroutine(cueSheet, null, cueSheet.acbFile, cueSheet.awbFile, loadAwbOnMemory));
		}
	}

	private IEnumerator RegisterAcfAsync(string acfFile, string dspBusSetting)
	{
	#if UNITY_EDITOR
		string acfPath = "file://" + CriWare.Common.streamingAssetsPath + "/" + acfFile;
	#else
		string acfPath = CriWare.Common.streamingAssetsPath + "/" + acfFile;
	#endif
		using (var req = new WWW(acfPath)) {
			yield return req;
			this.acfData = req.bytes;
		}
		/*
		* If there is already an allocated ACF data,
		* unregister and then release it.
		*/
		if (this.acfRegisterGCHandle.IsAllocated) {
			CriAtomEx.UnregisterAcf();
			this.acfRegisterGCHandle.Free();
		}
		this.acfRegisterGCHandle = GCHandle.Alloc(this.acfData, GCHandleType.Pinned);
		CriAtomEx.RegisterAcf(this.acfRegisterGCHandle.AddrOfPinnedObject(), acfData.Length);

		if (!String.IsNullOrEmpty(dspBusSetting)) {
			AttachDspBusSetting(dspBusSetting);
		}
		this.acfIsLoading = false;
	}
#endif


	private void LoadAcbFileAsync(CriAtomCueSheet cueSheet, CriFsBinder binder, string acbFile, string awbFile, bool loadAwbOnMemory)
	{
		if (String.IsNullOrEmpty(acbFile)) {
			return;
		}

		StartCoroutine(LoadAcbFileCoroutine(cueSheet, binder, acbFile, awbFile, loadAwbOnMemory));
	}

	private IEnumerator LoadAcbFileCoroutine(CriAtomCueSheet cueSheet, CriFsBinder binder, string acbPath, string awbPath, bool loadAwbOnMemory)
	{
		cueSheet.loaderStatus = CriAtomExAcbLoader.Status.Loading;

		if ((binder == null) && CriWare.Common.IsStreamingAssetsPath(acbPath)) {
			acbPath = Path.Combine(CriWare.Common.streamingAssetsPath, acbPath);
		}

		if (!String.IsNullOrEmpty(awbPath)) {
			if ((binder == null) && CriWare.Common.IsStreamingAssetsPath(awbPath)) {
				awbPath = Path.Combine(CriWare.Common.streamingAssetsPath, awbPath);
			}
		}

		while (this.acfIsLoading) {
			yield return null;
		}

		using (var asyncLoader = CriAtomExAcbLoader.LoadAcbFileAsync(binder, acbPath, awbPath, loadAwbOnMemory)) {
			if (asyncLoader == null) {
				cueSheet.loaderStatus = CriAtomExAcbLoader.Status.Error;
				yield break;
			}

			while (true) {
				var status = asyncLoader.GetStatus();
				cueSheet.loaderStatus = status;
				if (status == CriAtomExAcbLoader.Status.Complete) {
					cueSheet.acb = asyncLoader.MoveAcb();
					break;
				} else if (status == CriAtomExAcbLoader.Status.Error) {
					break;
				}
				yield return null;
			}
		}
	}

	private void LoadAcbDataAsync(CriAtomCueSheet cueSheet, byte[] acbData, CriFsBinder awbBinder, string awbFile, bool loadAwbOnMemory)
	{
		StartCoroutine(LoadAcbDataCoroutine(cueSheet, acbData, awbBinder, awbFile, loadAwbOnMemory));
	}

	private IEnumerator LoadAcbDataCoroutine(CriAtomCueSheet cueSheet, byte[] acbData, CriFsBinder awbBinder, string awbPath, bool loadAwbOnMemory)
	{
		cueSheet.loaderStatus = CriAtomExAcbLoader.Status.Loading;

		if (!String.IsNullOrEmpty(awbPath)) {
			if ((awbBinder == null) && CriWare.Common.IsStreamingAssetsPath(awbPath)) {
				awbPath = Path.Combine(CriWare.Common.streamingAssetsPath, awbPath);
			}
		}

		while (this.acfIsLoading) {
			yield return null;
		}

		using (var asyncLoader = CriAtomExAcbLoader.LoadAcbDataAsync(acbData, awbBinder, awbPath, loadAwbOnMemory)) {
			if (asyncLoader == null) {
				cueSheet.loaderStatus = CriAtomExAcbLoader.Status.Error;
				yield break;
			}

			while (true) {
				var status = asyncLoader.GetStatus();
				cueSheet.loaderStatus = status;
				if (status == CriAtomExAcbLoader.Status.Complete) {
					cueSheet.acb = asyncLoader.MoveAcb();
					break;
				} else if (status == CriAtomExAcbLoader.Status.Error) {
					break;
				}
				yield return null;
			}
		}
	}

	[AOT.MonoPInvokeCallback(typeof(CriAtomExSequencer.EventCbFunc))]
	public static void SequenceEventCallbackFromNative(string eventString)
	{
		/* 本関数はアプリケーションメインスレッドの CriAtom.Update から呼び出される */
		if (eventUserCbFunc != null) {
			eventUserCbFunc(eventString);
		}
	}

	[AOT.MonoPInvokeCallback(typeof(CriAtomExSequencer.EventCallback))]
	private static void SequenceCallbackFromNative(ref CriAtomExSequencer.CriAtomExSequenceEventInfo criAtomExSequenceInfo)
	{
		/* 本関数はアプリケーションメインスレッドの CriAtom.Update から呼び出される */
		if (eventUserCallback != null) {
			eventUserCallback(ref criAtomExSequenceInfo);
		}
	}

	[AOT.MonoPInvokeCallback(typeof(CriAtomExBeatSync.CbFunc))]
	public static void BeatSyncCallbackFromNative(ref CriAtomExBeatSync.Info info)
	{
		/* 本関数はアプリケーションメインスレッドの CriAtom.Update から呼び出される */
		if (beatsyncUserCbFunc != null) {
			beatsyncUserCbFunc(ref info);
		}
	}

	[AOT.MonoPInvokeCallback(typeof(CriAtomEx.CueLinkCbFunc))]
	public static void CueLinkCallbackFromNative(ref CriAtomEx.CueLinkInfo info)
	{
		/* 本関数はアプリケーションメインスレッドの CriAtom.Update から呼び出される */
		if (cueLinkUserCbFunc != null) {
			cueLinkUserCbFunc(ref info);
		}
	}

	/* プラグイン内部用API */
	public static void SetEventCallback(CriAtomExSequencer.EventCbFunc func, string separator)
	{
		/* ネイティブプラグインに関数ポインタを登録 */
		IntPtr ptr = IntPtr.Zero;
		eventUserCbFunc = func;
		if (func != null) {
			CriAtomExSequencer.EventCbFunc delegateObject;
			delegateObject = new CriAtomExSequencer.EventCbFunc(CriAtom.SequenceEventCallbackFromNative);
			ptr = Marshal.GetFunctionPointerForDelegate(delegateObject);
		}
		CriAtomPlugin.criAtomUnitySequencer_SetEventCallback(ptr, separator);
	}

	private static void RegisterEventCallbackChain(CriAtomExSequencer.EventCallback func)
	{
		/* Register callback function's pointer to native plugin side. */
		IntPtr ptr = IntPtr.Zero;
		if (eventUserCallback == null) {
			CriAtomExSequencer.EventCallback delegateObject;
			delegateObject = new CriAtomExSequencer.EventCallback(CriAtom.SequenceCallbackFromNative);
			ptr = Marshal.GetFunctionPointerForDelegate(delegateObject);
			CriAtomPlugin.criAtomUnitySequencer_SetCallback(ptr);
		}
		eventUserCallback += func;
	}

	private static void UnregisterEventCallbackChain(CriAtomExSequencer.EventCallback func)
	{
		eventUserCallback -= func;
		if (eventUserCallback == null) {
			CriAtomPlugin.criAtomUnitySequencer_SetCallback(IntPtr.Zero);
		}
	}

	public static void SetBeatSyncCallback(CriAtomExBeatSync.CbFunc func)
	{
		Debug.LogWarning("[CRIWARE] This function(CriAtom.SetBeatSyncCallback) is deprecated. Use CriAtomExBeatSync class.");
		if (func == null) {
			/* Only unregistered via CriAtomExBeatSync or CriAtom SetBeatSyncCallback(). */
			UnregisterBeatSyncCallbackChain(obsoleteBeatSyncFunc);
			obsoleteBeatSyncFunc = null;
			return;
		}
		if (obsoleteBeatSyncFunc != null) {
			UnregisterBeatSyncCallbackChain(obsoleteBeatSyncFunc);
		}
		obsoleteBeatSyncFunc = func;
		RegisterBeatSyncCallbackChain(obsoleteBeatSyncFunc);
	}

	private static void RegisterBeatSyncCallbackChain(CriAtomExBeatSync.CbFunc func)
	{
		if (beatsyncUserCbFunc == null) {
			CriAtomExBeatSync.CbFunc delegateObject;
			delegateObject = new CriAtomExBeatSync.CbFunc(CriAtom.BeatSyncCallbackFromNative);
			var ptr = Marshal.GetFunctionPointerForDelegate(delegateObject);
			CriAtomPlugin.criAtomUnity_SetBeatSyncCallback(ptr);
		}
		beatsyncUserCbFunc += func;
	}

	private static void UnregisterBeatSyncCallbackChain(CriAtomExBeatSync.CbFunc func)
	{
		beatsyncUserCbFunc -= func;
		if (beatsyncUserCbFunc == null) {
			CriAtomPlugin.criAtomUnity_SetBeatSyncCallback(IntPtr.Zero);
		}
	}

	private static void RegisterCueLinkCallbackChain(CriAtomEx.CueLinkCbFunc func)
	{
		if (cueLinkUserCbFunc == null) {
			CriAtomEx.CueLinkCbFunc delegateObject;
			delegateObject = new CriAtomEx.CueLinkCbFunc(CriAtom.CueLinkCallbackFromNative);
			var ptr = Marshal.GetFunctionPointerForDelegate(delegateObject);
			CriAtomPlugin.criAtomUnity_SetCueLinkCallback(ptr);
		}
		cueLinkUserCbFunc += func;
	}

	private static void UnregisterCueLinkCallbackChain(CriAtomEx.CueLinkCbFunc func)
	{
		cueLinkUserCbFunc -= func;
		if (cueLinkUserCbFunc == null) {
			CriAtomPlugin.criAtomUnity_SetCueLinkCallback(IntPtr.Zero);
		}
	}

	#endregion
	/* @endcond */

}

} //namespace CriWare
/** @} */
/* end of file */
