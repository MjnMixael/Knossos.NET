﻿using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Knossos.NET.Classes;
using Knossos.NET.Models;
using Knossos.NET.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Knossos.NET.ViewModels
{
    /// <summary>
    /// This is the class responsable for the "settings" tab
    /// </summary>
    public partial class GlobalSettingsViewModel : ViewModelBase
    {
        /* Limiters definition */
        private const long speedUnlimited = 0;
        private const long speedHalfMB = 850000;
        private const long speed1MB = 18000000;
        private const long speed2MB = 34000000;
        private const long speed3MB = 50000000;
        private const long speed4MB = 68000000;
        private const long speed5MB = 84000000;
        private const long speed6MB = 102000000;
        private const long speed7MB = 120000000;
        private const long speed8MB = 137000000;
        private const long speed9MB = 155000000;
        private const long speed10MB = 170000000;

        [ObservableProperty]
        internal bool blCfNebula = false;
        [ObservableProperty]
        internal bool blDlNebula = false;
        [ObservableProperty]
        internal bool blAigaion = false;

        [ObservableProperty]
        internal bool flagDataLoaded = false;
        [ObservableProperty]
        internal bool enable16BitColor = false;
        [ObservableProperty]
        internal bool windowsOS = false;

        /* Knossos */
        [ObservableProperty]
        internal string imgCacheSize = "0 MB";
        [ObservableProperty]
        internal string basePath = string.Empty;
        [ObservableProperty]
        internal bool enableLogFile = true;
        [ObservableProperty]
        internal int logLevel = 1;
        [ObservableProperty]
        internal bool fs2RootPack = false;
        [ObservableProperty]
        internal string numberOfMods = string.Empty;
        [ObservableProperty]
        internal string numberOfBuilds = string.Empty;
        [ObservableProperty]
        internal string detectedOS = string.Empty;
        [ObservableProperty]
        internal string cpuArch = string.Empty;
        [ObservableProperty]
        internal bool isAVX = false;
        [ObservableProperty]
        internal bool isAVX2 = false;
        [ObservableProperty]
        internal bool forceSSE2 = false;
        [ObservableProperty]
        internal int maxConcurrentSubtasks = 3;
        [ObservableProperty]
        internal long maxDownloadSpeedIndex = 0;
        [ObservableProperty]
        internal CompressionSettings modCompression = CompressionSettings.Manual;
        [ObservableProperty]
        internal int compressionMaxParallelism = 4;
        [ObservableProperty]
        internal bool checkUpdates = true;
        [ObservableProperty]
        internal bool autoUpdate = false;
        [ObservableProperty]
        internal bool deleteUploadedFiles = true;

        /*VIDEO*/
        [ObservableProperty]
        internal int bitsSelectedIndex = 0;
        [ObservableProperty]
        internal int resolutionSelectedIndex = 0;
        [ObservableProperty]
        internal int textureSelectedIndex = 0;
        internal ObservableCollection<ComboBoxItem> ResolutionItems { get; set; } = new ObservableCollection<ComboBoxItem>();
        [ObservableProperty]
        internal int shadowQualitySelectedIndex = 0;
        [ObservableProperty]
        internal int aaSelectedIndex = 5;
        [ObservableProperty]
        internal int msaaSelectedIndex = 0;
        [ObservableProperty]
        internal bool enableSoftParticles = true;
        [ObservableProperty]
        internal bool enableDeferredLighting = true;
        [ObservableProperty]
        internal int windowMode = 2;
        [ObservableProperty]
        internal bool vsync = true;
        [ObservableProperty]
        internal bool postProcess = true;
        [ObservableProperty]
        internal bool noFpsCapping = false;
        [ObservableProperty]
        internal bool showFps = false;

        /*AUDIO*/
        [ObservableProperty]
        internal int playbackSelectedIndex = 0;
        internal ObservableCollection<ComboBoxItem> PlaybackItems { get; set; } = new ObservableCollection<ComboBoxItem>();
        [ObservableProperty]
        internal int captureSelectedIndex = 0;
        internal ObservableCollection<ComboBoxItem> CaptureItems { get; set; } = new ObservableCollection<ComboBoxItem>();
        [ObservableProperty]
        internal int sampleRateSelectedIndex = 0;
        [ObservableProperty]
        internal bool enableEFX = false;
        [ObservableProperty]
        internal bool disableAudio = false;
        [ObservableProperty]
        internal bool disableMusic = false;
        [ObservableProperty]
        internal bool enableTTS = true;
        [ObservableProperty]
        internal int voiceSelectedIndex = 0;
        internal ObservableCollection<ComboBoxItem> VoiceItems { get; set; } = new ObservableCollection<ComboBoxItem>();
        [ObservableProperty]
        internal bool ttsTechroom = true;
        [ObservableProperty]
        internal bool ttsBriefings = true;
        [ObservableProperty]
        internal bool ttsIngame = true;
        [ObservableProperty]
        internal bool ttsMulti = true;
        [ObservableProperty]
        internal bool ttsDescription = true;
        [ObservableProperty]
        internal int ttsVolume = 100;
        [ObservableProperty]
        internal bool playingTTS = false;

        /*JOYSTICK*/
        internal ObservableCollection<ComboBoxItem> Joystick1Items { get; set; } = new ObservableCollection<ComboBoxItem>();
        internal ObservableCollection<ComboBoxItem> Joystick2Items { get; set; } = new ObservableCollection<ComboBoxItem>();
        internal ObservableCollection<ComboBoxItem> Joystick3Items { get; set; } = new ObservableCollection<ComboBoxItem>();
        internal ObservableCollection<ComboBoxItem> Joystick4Items { get; set; } = new ObservableCollection<ComboBoxItem>();
        [ObservableProperty]
        internal int joy1SelectedIndex = -1;
        [ObservableProperty]
        internal int joy2SelectedIndex = -1;
        [ObservableProperty]
        internal int joy3SelectedIndex = -1;
        [ObservableProperty]
        internal int joy4SelectedIndex = -1;

        /* MOD / FS2 */
        [ObservableProperty]
        internal string globalCmd = string.Empty;
        [ObservableProperty]
        internal int fs2LangSelectedIndex = 0;
        [ObservableProperty]
        internal uint multiPort = 7808;
        [ObservableProperty]
        internal uint mouseSensitivity = 5;
        [ObservableProperty]
        internal uint joystickSensitivity = 9;
        [ObservableProperty]
        internal uint joystickDeadZone = 10;

        public GlobalSettingsViewModel()
        {
        }

        /// <summary>
        /// Loads data from the GlobalSettings.cs class into this one to display it in the UI
        /// Also loads flag data from a FSO build, if one is installed
        /// </summary>
        public void LoadData()
        {
            var old_path = KnUtils.GetFSODataFolderPath();
            var flagData = GetFlagData();

            // reset the ini info if we have gotten an updated preferred path from FSO.
            if (old_path != KnUtils.GetFSODataFolderPath()){
                Knossos.globalSettings.Load();
            }
            /* Knossos Settings */
            if (Knossos.globalSettings.basePath != null)
            {
                BasePath = Knossos.globalSettings.basePath;
            }
            EnableLogFile = Knossos.globalSettings.enableLogFile;
            LogLevel= Knossos.globalSettings.logLevel;
            Fs2RootPack = Knossos.retailFs2RootFound;
            NumberOfMods = Knossos.GetInstalledModList(null).Count.ToString();
            NumberOfBuilds = Knossos.GetInstalledBuildsList(null).Count.ToString();
            if(KnUtils.IsWindows)
            {
                DetectedOS = "Windows";
                WindowsOS = true;
            }
            else
            {
                if(KnUtils.IsLinux)
                {
                    DetectedOS = "Linux";
                }
                else
                {
                    if(KnUtils.IsMacOS)
                    {
                        DetectedOS = "OSX";
                    }
                }
            }

            CpuArch = KnUtils.CpuArch;
            IsAVX = KnUtils.CpuAVX;
            IsAVX2 = KnUtils.CpuAVX2;
            ForceSSE2 = Knossos.globalSettings.forceSSE2;
            MaxConcurrentSubtasks = Knossos.globalSettings.maxConcurrentSubtasks - 1;
            switch(Knossos.globalSettings.maxDownloadSpeed)
            {
                case speedUnlimited: MaxDownloadSpeedIndex = 0; break;
                case speedHalfMB: MaxDownloadSpeedIndex = 1; break;
                case speed1MB: MaxDownloadSpeedIndex = 2; break;
                case speed2MB: MaxDownloadSpeedIndex = 3; break;
                case speed3MB: MaxDownloadSpeedIndex = 4; break;
                case speed4MB: MaxDownloadSpeedIndex = 5; break;
                case speed5MB: MaxDownloadSpeedIndex = 6; break;
                case speed6MB: MaxDownloadSpeedIndex = 7; break;
                case speed7MB: MaxDownloadSpeedIndex = 8; break;
                case speed8MB: MaxDownloadSpeedIndex = 9; break;
                case speed9MB: MaxDownloadSpeedIndex = 10; break;
                case speed10MB: MaxDownloadSpeedIndex = 11; break;
                default: MaxDownloadSpeedIndex = 0; break;
            }

            if (Knossos.globalSettings.mirrorBlacklist != null)
            {
                if (Knossos.globalSettings.mirrorBlacklist.Contains("dl.fsnebula.org"))
                {
                    BlDlNebula = true;
                }
                if (Knossos.globalSettings.mirrorBlacklist.Contains("cf.fsnebula.org"))
                {
                    BlCfNebula = true;
                }
                if (Knossos.globalSettings.mirrorBlacklist.Contains("aigaion.feralhosting.com"))
                {
                    BlAigaion = true;
                }
            }

            ModCompression = Knossos.globalSettings.modCompression;
            CompressionMaxParallelism = Knossos.globalSettings.compressionMaxParallelism;
            CheckUpdates = Knossos.globalSettings.checkUpdate;
            AutoUpdate = Knossos.globalSettings.autoUpdate;
            DeleteUploadedFiles = Knossos.globalSettings.deleteUploadedFiles;

            /* VIDEO SETTINGS */
            //RESOLUTION
            ResolutionItems.Clear();
            if (flagData != null && flagData.displays != null)
            {
                foreach(var display in flagData.displays)
                {
                    var item = new ComboBoxItem();
                    item.Content = display.name;
                    item.IsEnabled = false;
                    item.Tag = 0;
                    ResolutionItems.Add(item);
                    if(display.modes != null)
                    {
                        foreach(var mode in display.modes)
                        {
                            var itemMode = new ComboBoxItem();
                            itemMode.Content = mode.x+"x"+mode.y;
                            itemMode.Tag = display.index;
                            ResolutionItems.Add(itemMode);
                            if(mode.bits == 16)
                            {
                                Enable16BitColor = true;
                            }
                        }
                    }

                    if(Knossos.globalSettings.displayResolution == null)
                    {
                        Knossos.globalSettings.displayResolution = display.width + "x" + display.height;
                    }
                }
            }
            var resoItem = ResolutionItems.FirstOrDefault(i => i.Content?.ToString() == Knossos.globalSettings.displayResolution && i.Tag?.ToString() == Knossos.globalSettings.displayIndex.ToString());
            if (resoItem != null)
            {
                var index = ResolutionItems.IndexOf(resoItem);
                if(index != -1)
                {
                    ResolutionSelectedIndex = index;
                }
            }
            else
            {
                ResolutionSelectedIndex = 0;
            }
            //COLOR DEPTH
            if(Knossos.globalSettings.displayColorDepth == 32)
            {
                BitsSelectedIndex = 0;
            }
            else
            {
                BitsSelectedIndex = 1;
            }
            //Texture Filter
            TextureSelectedIndex = Knossos.globalSettings.textureFilter;
            //Shadows
            ShadowQualitySelectedIndex = Knossos.globalSettings.shadowQuality;
            //AA
            AaSelectedIndex = Knossos.globalSettings.aaPreset;
            //MSAA
            MsaaSelectedIndex = Knossos.globalSettings.msaaPreset;
            //SoftParticles
            EnableSoftParticles = Knossos.globalSettings.enableSoftParticles;
            //DeferredLighting
            EnableDeferredLighting = Knossos.globalSettings.enableDeferredLighting;
            //WindowMode
            WindowMode = Knossos.globalSettings.windowMode;
            //VSYNC
            Vsync = Knossos.globalSettings.vsync;
            //No Post Process
            PostProcess = Knossos.globalSettings.postProcess;
            //No FPS Cap
            NoFpsCapping = Knossos.globalSettings.noFpsCapping;
            //FPS
            ShowFps = Knossos.globalSettings.showFps;

            /* AUDIO SETTINGS */
            //Playback Devices
            PlaybackItems.Clear();
            if (flagData != null && flagData.openal != null && flagData.openal.playback_devices != null)
            {
                foreach (var playback in flagData.openal.playback_devices)
                {
                    var item = new ComboBoxItem();
                    item.Content = playback;
                    item.Tag = playback;
                    PlaybackItems.Add(item);

                    if (Knossos.globalSettings.playbackDevice == null)
                    {
                        Knossos.globalSettings.playbackDevice = flagData.openal.default_playback;
                    }
                }
            }
            var pbItem = PlaybackItems.FirstOrDefault(i => i.Tag?.ToString() == Knossos.globalSettings.playbackDevice);
            if (pbItem != null)
            {
                var index = PlaybackItems.IndexOf(pbItem);
                if (index != -1)
                {
                    PlaybackSelectedIndex = index;
                }
            }
            else
            {
                PlaybackSelectedIndex = 0;
            }
            //Capture Devices
            CaptureItems.Clear();
            if (flagData != null && flagData.openal != null && flagData.openal.capture_devices != null)
            {
                foreach (var capture in flagData.openal.capture_devices)
                {
                    var item = new ComboBoxItem();
                    item.Content = capture;
                    item.Tag = capture;
                    CaptureItems.Add(item);

                    if (Knossos.globalSettings.captureDevice == null)
                    {
                        Knossos.globalSettings.captureDevice = flagData.openal.default_capture;
                    }
                }
            }
            var ctItem = CaptureItems.FirstOrDefault(i => i.Tag?.ToString() == Knossos.globalSettings.captureDevice);
            if (ctItem != null)
            {
                var index = CaptureItems.IndexOf(ctItem);
                if (index != -1)
                {
                    CaptureSelectedIndex = index;
                }
            }
            else
            {
                CaptureSelectedIndex = 0;
            }
            //Sample Rate
            switch (Knossos.globalSettings.sampleRate)
            {
                case 44100: SampleRateSelectedIndex = 0; break;
                case 48000: SampleRateSelectedIndex = 1; break;
                case 96000: SampleRateSelectedIndex = 2; break;
                case 192000: SampleRateSelectedIndex = 3; break;
                default: SampleRateSelectedIndex = 0; break;
            }
            //Enable EFX
            EnableEFX = Knossos.globalSettings.enableEfx;
            //Disable Audio
            DisableAudio = Knossos.globalSettings.disableAudio;
            //Disable Music
            DisableAudio = Knossos.globalSettings.disableMusic;
            //TTS Settings
            EnableTTS = Knossos.globalSettings.enableTts;
            TtsBriefings = Knossos.globalSettings.ttsBriefings;
            TtsTechroom = Knossos.globalSettings.ttsTechroom;
            TtsIngame = Knossos.globalSettings.ttsIngame;
            TtsMulti = Knossos.globalSettings.ttsMulti;
            TtsVolume = Knossos.globalSettings.ttsVolume;
            TtsDescription = Knossos.globalSettings.ttsDescription;
            VoiceItems.Clear();
            if (flagData != null && flagData.voices != null)
            {
                foreach (var voice in flagData.voices)
                {
                    var item = new ComboBoxItem();
                    item.Content = voice;
                    item.Tag = voice;
                    VoiceItems.Add(item);
                }
            }
            if (Knossos.globalSettings.ttsVoice == null)
            {
                Knossos.globalSettings.ttsVoice = 0;
            }
            else
            {
                if(Knossos.globalSettings.ttsVoice.Value + 1 <= VoiceItems.Count)
                {
                    VoiceSelectedIndex = Knossos.globalSettings.ttsVoice.Value;
                }
            }

            //Joysticks
            //The reason for this BS is that i cant re-use comboBox items in multiple controls
            Joystick1Items.Clear();
            Joystick2Items.Clear();
            Joystick3Items.Clear();
            Joystick4Items.Clear();
            Joy1SelectedIndex = -1;
            Joy2SelectedIndex = -1;
            Joy3SelectedIndex = -1;
            Joy4SelectedIndex = -1;
            var noJoyItem = new ComboBoxItem();
            noJoyItem.Content = "No Joystick";
            noJoyItem.Tag = null;
            Joystick1Items.Add(noJoyItem);
            var noJoy2Item = new ComboBoxItem();
            noJoy2Item.Content = "No Joystick";
            noJoy2Item.Tag = null;
            Joystick2Items.Add(noJoy2Item);
            var noJoy3Item = new ComboBoxItem();
            noJoy3Item.Content = "No Joystick";
            noJoy3Item.Tag = null;
            Joystick3Items.Add(noJoy3Item);
            var noJoy4Item = new ComboBoxItem();
            noJoy4Item.Content = "No Joystick";
            noJoy4Item.Tag = null;
            Joystick4Items.Add(noJoy4Item);
            Joy1SelectedIndex = 0;
            Joy2SelectedIndex = 0;
            Joy3SelectedIndex = 0;
            Joy4SelectedIndex = 0;

            if (flagData != null && flagData.joysticks != null)
            {
                foreach (var joy in flagData.joysticks)
                {
                    var item = new ComboBoxItem();
                    item.Content = joy.name + " - GUID: " + joy.guid + " - ID: " + joy.id;
                    item.Tag = joy;
                    var item2 = new ComboBoxItem();
                    item2.Content = joy.name + " - GUID: " + joy.guid + " - ID: " + joy.id; ;
                    item2.Tag = joy;
                    var item3 = new ComboBoxItem();
                    item3.Content = joy.name + " - GUID: " + joy.guid + " - ID: " + joy.id; ;
                    item3.Tag = joy;
                    var item4 = new ComboBoxItem();
                    item4.Content = joy.name + " - GUID: " + joy.guid + " - ID: " + joy.id; ;
                    item4.Tag = joy;
                    Joystick1Items.Add(item);
                    Joystick2Items.Add(item2);
                    Joystick3Items.Add(item3);
                    Joystick4Items.Add(item4);
                }

                // i hate this
                if(Knossos.globalSettings.joystick1 != null)
                {
                    bool found = false;
                    foreach(var item in Joystick1Items)
                    {
                        if(item.Tag != null)
                        {
                            var joystick = (Joystick)item.Tag;
                            if (joystick.guid == Knossos.globalSettings.joystick1.guid)
                            {
                                var index = Joystick1Items.IndexOf(item);
                                if (index != -1)
                                {
                                    Joy1SelectedIndex = index;
                                    found = true;
                                }
                            }
                        }
                    }

                    if(!found)
                    {
                        var missingItem = new ComboBoxItem();
                        missingItem.Content = "(Missing) " + Knossos.globalSettings.joystick1.name + " GUID: " + Knossos.globalSettings.joystick1.guid + " ID: " + Knossos.globalSettings.joystick1.id;
                        missingItem.Tag = Knossos.globalSettings.joystick1;
                        Joystick1Items.Add(missingItem);
                        var index = Joystick1Items.IndexOf(missingItem);
                        if (index != -1)
                        {
                            Joy1SelectedIndex = index;
                        }
                    }
                }
                if (Knossos.globalSettings.joystick2 != null)
                {
                    bool found = false;
                    foreach (var item in Joystick2Items)
                    {
                        if (item.Tag != null)
                        {
                            var joystick = (Joystick)item.Tag;
                            if (joystick.guid == Knossos.globalSettings.joystick2.guid)
                            {
                                var index = Joystick2Items.IndexOf(item);
                                if (index != -1)
                                {
                                    Joy2SelectedIndex = index;
                                    found = true;
                                }
                            }
                        }
                    }

                    if (!found)
                    {
                        var missingItem = new ComboBoxItem();
                        missingItem.Content = "(Missing) " + Knossos.globalSettings.joystick2.name + " GUID: " + Knossos.globalSettings.joystick2.guid + " ID: " + Knossos.globalSettings.joystick2.id;
                        missingItem.Tag = Knossos.globalSettings.joystick2;
                        Joystick2Items.Add(missingItem);
                        var index = Joystick2Items.IndexOf(missingItem);
                        if (index != -1)
                        {
                            Joy2SelectedIndex = index;
                        }
                    }
                }
                if (Knossos.globalSettings.joystick3 != null)
                {
                    bool found = false;
                    foreach (var item in Joystick3Items)
                    {
                        if (item.Tag != null)
                        {
                            var joystick = (Joystick)item.Tag;
                            if (joystick.guid == Knossos.globalSettings.joystick3.guid)
                            {
                                var index = Joystick3Items.IndexOf(item);
                                if (index != -1)
                                {
                                    Joy3SelectedIndex = index;
                                    found = true;
                                }
                            }
                        }
                    }

                    if (!found)
                    {
                        var missingItem = new ComboBoxItem();
                        missingItem.Content = "(Missing) " + Knossos.globalSettings.joystick3.name + " GUID: " + Knossos.globalSettings.joystick3.guid + " ID: " + Knossos.globalSettings.joystick3.id;
                        missingItem.Tag = Knossos.globalSettings.joystick3;
                        Joystick3Items.Add(missingItem);
                        var index = Joystick3Items.IndexOf(missingItem);
                        if (index != -1)
                        {
                            Joy3SelectedIndex = index;
                        }
                    }
                }
                if (Knossos.globalSettings.joystick4 != null)
                {
                    bool found = false;
                    foreach (var item in Joystick4Items)
                    {
                        if (item.Tag != null)
                        {
                            var joystick = (Joystick)item.Tag;
                            if (joystick.guid == Knossos.globalSettings.joystick4.guid)
                            {
                                var index = Joystick4Items.IndexOf(item);
                                if (index != -1)
                                {
                                    Joy4SelectedIndex = index;
                                    found = true;
                                }
                            }
                        }
                    }

                    if (!found)
                    {
                        var missingItem = new ComboBoxItem();
                        missingItem.Content = "(Missing) " + Knossos.globalSettings.joystick4.name + " GUID: " + Knossos.globalSettings.joystick4.guid + " ID: " + Knossos.globalSettings.joystick4.id;
                        missingItem.Tag = Knossos.globalSettings.joystick4;
                        Joystick4Items.Add(missingItem);
                        var index = Joystick4Items.IndexOf(missingItem);
                        if (index != -1)
                        {
                            Joy4SelectedIndex = index;
                        }
                    }
                }
            }

            JoystickDeadZone = Knossos.globalSettings.joystickDeadZone;
            JoystickSensitivity = Knossos.globalSettings.joystickSensitivity;
            MouseSensitivity = Knossos.globalSettings.mouseSensitivity;

            /* MOD SETTINGS */

            //GLOBAL CMD
            if(Knossos.globalSettings.globalCmdLine != null)
            {
                GlobalCmd = Knossos.globalSettings.globalCmdLine;
            }

            //FS2 Lang
            switch(Knossos.globalSettings.fs2Lang)
            {
                case "English": Fs2LangSelectedIndex = 0; break;
                case "German": Fs2LangSelectedIndex = 1; break;
                case "French": Fs2LangSelectedIndex = 2; break;
                case "Polish": Fs2LangSelectedIndex = 3; break;
                default: Fs2LangSelectedIndex = 0;  break;
            }

            //Multi Port
            MultiPort = Knossos.globalSettings.multiPort;
        }

        private FlagsJsonV1? GetFlagData()
        {
            FlagDataLoaded = false;
            var builds = Knossos.GetInstalledBuildsList();
            if (builds.Any())
            {
                //First the stable ones
                var stables = builds.Where(b => b.stability == FsoStability.Stable).ToList();
                if (stables.Any())
                {
                    stables.Sort(FsoBuild.CompareVersion);
                    foreach (var stable in stables)
                    {
                        var flags = stable.GetFlagsV1();
                        if (flags != null)
                        {
                            FlagDataLoaded = true;
                            Knossos.flagDataLoaded = true;
                            KnUtils.SetFSODataFolderPath(flags.pref_path);
                            return flags;
                        }
                    }
                }

                //If we are still here try all others
                var others = builds.Where(b => b.stability != FsoStability.Stable);
                if (others.Any())
                {
                    foreach (var other in others)
                    {
                        var flags = other.GetFlagsV1();
                        if (flags != null)
                        {
                            FlagDataLoaded = true;
                            Knossos.flagDataLoaded = true;
                            KnUtils.SetFSODataFolderPath(flags.pref_path);
                            return flags;
                        }
                    }
                }
            }

            Log.Add(Log.LogSeverity.Warning, "GlobalSettingsViewModel.GetFlagData()", "Unable to find a valid build to get flag data for global settings.");
            return null;
        }

        /* UI Buttons */
        /// <summary>
        /// Changes the knossos library path, reloads settings and nebula repo
        /// </summary>
        internal async void BrowseFolderCommand()
        {
            if (MainWindow.instance != null)
            {

                FolderPickerOpenOptions options = new FolderPickerOpenOptions(); 
                if (BasePath != string.Empty)
                { 
                    options.SuggestedStartLocation = await MainWindow.instance.StorageProvider.TryGetFolderFromPathAsync(BasePath);
                }
                options.AllowMultiple = false;

                var result = await MainWindow.instance.StorageProvider.OpenFolderPickerAsync(options);

                if (result != null && result.Count > 0)
                {
                    Knossos.globalSettings.basePath = result[0].Path.LocalPath.ToString();
                    Knossos.globalSettings.Save();
                    Knossos.ResetBasePath();
                    LoadData();
                }
            }
        }

        /// <summary>
        /// Reload data from json
        /// </summary>
        internal void ResetCommand()
        {
            var pxoUser = Knossos.globalSettings.pxoLogin;
            var pxoPassword = Knossos.globalSettings.pxoPassword;
            Knossos.globalSettings = new GlobalSettings();
            LoadData();
            Knossos.globalSettings.pxoPassword = pxoPassword;
            Knossos.globalSettings.pxoLogin = pxoUser;
            SaveCommand();
        }

        /// <summary>
        /// Copies data from the UI into the GlobalSettings.cs class and saves it into the json
        /// </summary>
        internal void SaveCommand()
        {
            /* Knossos Settings */
            if (BasePath != string.Empty)
            {
                Knossos.globalSettings.basePath = BasePath;
            }
            Knossos.globalSettings.enableLogFile = EnableLogFile;
            Knossos.globalSettings.logLevel = LogLevel;
            Knossos.globalSettings.forceSSE2 = ForceSSE2;
            Knossos.globalSettings.maxConcurrentSubtasks = MaxConcurrentSubtasks+1;
            switch (MaxDownloadSpeedIndex)
            {
                case 0: Knossos.globalSettings.maxDownloadSpeed = speedUnlimited; break;
                case 1: Knossos.globalSettings.maxDownloadSpeed = speedHalfMB; break;
                case 2: Knossos.globalSettings.maxDownloadSpeed = speed1MB; break;
                case 3: Knossos.globalSettings.maxDownloadSpeed = speed2MB; break;
                case 4: Knossos.globalSettings.maxDownloadSpeed = speed3MB; break;
                case 5: Knossos.globalSettings.maxDownloadSpeed = speed4MB; break;
                case 6: Knossos.globalSettings.maxDownloadSpeed = speed5MB; break;
                case 7: Knossos.globalSettings.maxDownloadSpeed = speed6MB; break;
                case 8: Knossos.globalSettings.maxDownloadSpeed = speed7MB; break;
                case 9: Knossos.globalSettings.maxDownloadSpeed = speed8MB; break;
                case 10: Knossos.globalSettings.maxDownloadSpeed = speed9MB; break;
                case 11: Knossos.globalSettings.maxDownloadSpeed = speed10MB; break;
            }

            List<string> blMirrors = new List<string>();
            if (BlDlNebula)
            {
                blMirrors.Add("dl.fsnebula.org");
            }
            if (BlCfNebula)
            {
                blMirrors.Add("cf.fsnebula.org");
            }
            if (BlAigaion)
            {
                blMirrors.Add("aigaion.feralhosting.com");
            }
            if(blMirrors.Any() && blMirrors.Count() != 3 /*Invalid!*/)
            {
                Knossos.globalSettings.mirrorBlacklist = blMirrors.ToArray();
            }
            else
            {
                Knossos.globalSettings.mirrorBlacklist = null;
                BlDlNebula = false;
                BlCfNebula = false;
                BlAigaion = false;
            }

            Knossos.globalSettings.modCompression = ModCompression;
            Knossos.globalSettings.compressionMaxParallelism = CompressionMaxParallelism;
            Knossos.globalSettings.checkUpdate = CheckUpdates;
            Knossos.globalSettings.deleteUploadedFiles = DeleteUploadedFiles;
            if(!CheckUpdates)
            {
                AutoUpdate = false;
            }
            Knossos.globalSettings.autoUpdate = AutoUpdate;

            /* VIDEO */
            //Resolution
            if (ResolutionSelectedIndex + 1 <= ResolutionItems.Count)
            {
                Knossos.globalSettings.displayResolution = ResolutionItems[ResolutionSelectedIndex].Content?.ToString();
                var displayIndex = ResolutionItems[ResolutionSelectedIndex].Tag;
                if ( displayIndex != null)
                { 
                    Knossos.globalSettings.displayIndex = (int)displayIndex;
                }
            }
            //Color Depth
            if(BitsSelectedIndex == 0)
            {
                Knossos.globalSettings.displayColorDepth = 32;
            }
            else
            {
                Knossos.globalSettings.displayColorDepth = 16;
            }
            //Texture Filter
            Knossos.globalSettings.textureFilter = TextureSelectedIndex;
            //Shadows
            Knossos.globalSettings.shadowQuality = ShadowQualitySelectedIndex;
            //AA
            Knossos.globalSettings.aaPreset = AaSelectedIndex;
            //MSAA
            Knossos.globalSettings.msaaPreset = MsaaSelectedIndex;
            //SoftParticles
            Knossos.globalSettings.enableSoftParticles = EnableSoftParticles;
            //DeferredLighting
            Knossos.globalSettings.enableDeferredLighting = EnableDeferredLighting;
            //WindowMode
            Knossos.globalSettings.windowMode = WindowMode;
            //VSYNC
            Knossos.globalSettings.vsync = Vsync;
            //No Post Process
            Knossos.globalSettings.postProcess = PostProcess;
            //No FPS Cap
            Knossos.globalSettings.noFpsCapping = NoFpsCapping;
            //FPS
            Knossos.globalSettings.showFps = ShowFps;

            /* AUDIO SETTINGS */
            //Playback
            if (PlaybackSelectedIndex + 1 <= PlaybackItems.Count)
            {
                Knossos.globalSettings.playbackDevice = PlaybackItems[PlaybackSelectedIndex].Tag?.ToString();
            }
            //Capture
            if (CaptureSelectedIndex + 1 <= CaptureItems.Count)
            {
                Knossos.globalSettings.captureDevice = CaptureItems[CaptureSelectedIndex].Tag?.ToString();
            }
            //Sample Rate
            switch (SampleRateSelectedIndex)
            {
                case 0: Knossos.globalSettings.sampleRate = 44100; break;
                case 1: Knossos.globalSettings.sampleRate = 48000; break;
                case 2: Knossos.globalSettings.sampleRate = 96000; break;
                case 3: Knossos.globalSettings.sampleRate = 192000; break;
            }
            //Enable EFX
            Knossos.globalSettings.enableEfx = EnableEFX;
            //Disable Audio
            Knossos.globalSettings.disableAudio = DisableAudio;
            //Disable Music
            Knossos.globalSettings.disableMusic = DisableAudio;
            //TTS Settings
            Knossos.globalSettings.enableTts = EnableTTS;
            Knossos.globalSettings.ttsBriefings = TtsBriefings;
            Knossos.globalSettings.ttsTechroom = TtsTechroom;
            Knossos.globalSettings.ttsIngame = TtsIngame;
            Knossos.globalSettings.ttsMulti = TtsMulti;
            Knossos.globalSettings.ttsDescription = TtsDescription;
            Knossos.globalSettings.ttsVoice = VoiceSelectedIndex;
            Knossos.globalSettings.ttsVolume = TtsVolume;
            if (VoiceSelectedIndex >= 0 && VoiceItems.Count() > VoiceSelectedIndex && VoiceItems[VoiceSelectedIndex].Tag != null)
            {
                Knossos.globalSettings.ttsVoiceName = VoiceItems[VoiceSelectedIndex].Tag!.ToString();
            }

            /* JOYSTICKS */
            if (Joy1SelectedIndex + 1 <= Joystick1Items.Count && Joy1SelectedIndex != -1)
            {
                Knossos.globalSettings.joystick1 = (Joystick?)Joystick1Items[Joy1SelectedIndex].Tag;
            }
            else
            {
                Knossos.globalSettings.joystick1 = null;
            }
            if (Joy2SelectedIndex + 1 <= Joystick2Items.Count && Joy2SelectedIndex != -1 )
            {
                Knossos.globalSettings.joystick2 = (Joystick?)Joystick2Items[Joy2SelectedIndex].Tag;
            }
            else
            {
                Knossos.globalSettings.joystick2 = null;
            }
            if (Joy3SelectedIndex + 1 <= Joystick3Items.Count && Joy3SelectedIndex != -1)
            {
                Knossos.globalSettings.joystick3 = (Joystick?)Joystick3Items[Joy3SelectedIndex].Tag;
            }
            else
            {
                Knossos.globalSettings.joystick3 = null;
            }
            if (Joy4SelectedIndex + 1 <= Joystick4Items.Count && Joy4SelectedIndex != -1)
            {
                Knossos.globalSettings.joystick4 = (Joystick?)Joystick4Items[Joy4SelectedIndex].Tag;
            }
            else
            {
                Knossos.globalSettings.joystick4 = null;
            }

            Knossos.globalSettings.joystickDeadZone = JoystickDeadZone;
            Knossos.globalSettings.joystickSensitivity = JoystickSensitivity;
            Knossos.globalSettings.mouseSensitivity = MouseSensitivity;

            /* MOD SETTINGS */

            //GLOBAL CMD
            if (GlobalCmd.Trim().Length > 0)
            {
                Knossos.globalSettings.globalCmdLine = GlobalCmd;
            }
            else
            {
                Knossos.globalSettings.globalCmdLine = null;
            }

            //FS2 Lang
            switch (Fs2LangSelectedIndex)
            {
                case 0: Knossos.globalSettings.fs2Lang = "English"; break;
                case 1: Knossos.globalSettings.fs2Lang = "German"; break;
                case 2: Knossos.globalSettings.fs2Lang = "French"; break;
                case 3: Knossos.globalSettings.fs2Lang = "Polish"; break;
            }

            //Multi port
            Knossos.globalSettings.multiPort = MultiPort;

            Knossos.globalSettings.Save();
        }

        /// <summary>
        /// Start TTS Voice Test with selected voice
        /// </summary>
        internal void TestVoiceCommand()
        {
            if (VoiceSelectedIndex != -1)
            {
                string? voice_name = null;
                if (VoiceSelectedIndex >= 0 && VoiceItems.Count() > VoiceSelectedIndex && VoiceItems[VoiceSelectedIndex].Tag != null)
                {
                    voice_name = VoiceItems[VoiceSelectedIndex].Tag!.ToString();
                }
                PlayingTTS = true;
                Knossos.Tts("Developed in a joint operation by the Vasudan and Terran governments, the GTF Ulysses is an excellent all-around fighter. It offers superior maneuverability and a high top speed.", VoiceSelectedIndex, voice_name, TtsVolume, TTSCompletedCallback);
            }
        }

        /// <summary>
        /// Stop TTS
        /// </summary>
        internal void StopTTS()
        {
            Knossos.Tts(string.Empty);
        }

        /// <summary>
        /// When TTS test is over, change the button
        /// </summary>
        /// <returns></returns>
        private bool TTSCompletedCallback()
        {
            PlayingTTS = false;
            return true;
        }

        /// <summary>
        /// Opens the hard light wiki CMDline reference help
        /// </summary>
        internal void GlobalCmdHelp()
        {
            KnUtils.OpenBrowserURL("https://wiki.hard-light.net/index.php/Command-Line_Reference");
        }

        /// <summary>
        /// Reloads configuration and FSO flag data
        /// </summary>
        internal void ReloadFlagData()
        {
            Knossos.globalSettings.Load();
            LoadData();
        }

        /// <summary>
        /// Opens performance help window
        /// </summary>
        internal async void OpenPerformanceHelp()
        {
            if (MainWindow.instance != null)
            {
                var dialog = new PerformanceHelpView();

                await dialog.ShowDialog<PerformanceHelpView?>(MainWindow.instance);
            }
        }

        /// <summary>
        /// Open the GetSapiVoices window
        /// </summary>
        internal async void OpenGetVoices()
        {
            if (MainWindow.instance != null)
            {
                var dialog = new AddSapiVoicesView();
                dialog.DataContext = new AddSapiVoicesViewModel();

                await dialog.ShowDialog<AddSapiVoicesView?>(MainWindow.instance);
            }
        }

        /// <summary>
        /// Open the retails FS2 installer window
        /// </summary>
        internal async void InstallFS2Command()
        {
            if (MainWindow.instance != null)
            {
                var dialog = new Fs2InstallerView();
                dialog.DataContext = new Fs2InstallerViewModel();

                await dialog.ShowDialog<Fs2InstallerView?>(MainWindow.instance);
            }
        }

        /// <summary>
        /// Opens the quick setup guide window
        /// </summary>
        internal void QuickSetupCommand()
        {
            Knossos.OpenQuickSetup();
        }
        
        /// <summary>
        /// Opens the library cleaner window
        /// </summary>
        internal async void CleanupLibraryCommand()
        {
            if (MainWindow.instance != null)
            {
                var dialog = new CleanupKnossosLibraryView();
                dialog.DataContext = new CleanupKnossosLibraryViewModel();

                await dialog.ShowDialog<CleanupKnossosLibraryView?>(MainWindow.instance);
            }
        }

        /// <summary>
        /// Clears the knet image cache folder
        /// </summary>
        internal async void ClearImageCache()
        {
            await Task.Run(() => {
                try
                {
                    var path = KnUtils.GetImageCachePath();
                    Directory.Delete(path, true);
                    UpdateImgCacheSize();
                
                }
                catch (Exception ex)
                {
                    Log.Add(Log.LogSeverity.Error,"GlobalSettingsViewModel.ClearImageCache()",ex);
                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Update the size of the Knet image cache folder into UI
        /// </summary>
        public void UpdateImgCacheSize()
        {
            Task.Run(async () => {
                try
                {
                    var path = KnUtils.GetImageCachePath();
                    if (Directory.Exists(path))
                    {
                        var sizeInBytes = await KnUtils.GetSizeOfFolderInBytes(path).ConfigureAwait(false);
                        Dispatcher.UIThread.Invoke(()=>{ 
                            ImgCacheSize = KnUtils.FormatBytes(sizeInBytes);
                        });
                    }
                    else
                    {
                        Dispatcher.UIThread.Invoke(() => {
                            ImgCacheSize = "0 MB";
                        });
                    }
                }
                catch (Exception ex)
                {
                    Log.Add(Log.LogSeverity.Error, "GlobalSettingsViewModel.ClearImageCache()", ex);
                }
            }).ConfigureAwait(false);
        }
    }
}
