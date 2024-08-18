using System;
using System.Reflection;
using HarmonyLib;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Generics;
using VRage.Plugins;

namespace ParticleLimitRemover
{
    public class Plugin : IPlugin, IDisposable
    {
        public const string Name = "Particle Limit Remover";
        public static Plugin Instance { get; private set; }
        private static FieldInfo _effectsPool = typeof(MyParticlesManager).GetField("EffectsPool", BindingFlags.NonPublic | BindingFlags.Static);
        private static bool setup = false;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        public void Init(object gameInstance)
        {
            Instance = this;
            Config.LoadConfig();
            var value = _effectsPool.GetValue(null);
            value = new MyDynamicObjectPool<MyParticleEffect>(Config.CurrentConfig.Limit, delegate (MyParticleEffect x) { x.Clear(); });
        }

        public void Dispose()
        {
            Config.SaveConfig();
            Instance = null;
        }

        public void Update()
        {
            if (MyAPIGateway.Multiplayer == null || MySession.Static?.LocalCharacter == null)
            {
                return; 
            }

            if (!setup)
            {
                InitChatCommands();
            }
        }

        public static void InitChatCommands()
        {
            MyAPIGateway.Utilities.MessageEnteredSender -= HandleCommand;
            MyAPIGateway.Utilities.MessageEnteredSender += HandleCommand;
        }

        private static void HandleCommand(ulong sender, string messageText, ref bool sendToOthers)
        {
            if (messageText.ToLower().StartsWith("/plr"))
            {
                sendToOthers = false;
                var newlimit = messageText.Split(' ')[1];
                var newLimitInt = Convert.ToInt32(newlimit);
                Config.CurrentConfig.Limit = newLimitInt;
                MyAPIGateway.Utilities.ShowMessage("Particle Limit Remover", $"Set Particle Limit To {newLimitInt.ToString()}");
                Config.SaveConfig();
            }
        }
    }
}