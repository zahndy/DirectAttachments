using Elements.Core;
using FrooxEngine;
using FrooxEngine.CommonAvatar;
using HarmonyLib;
using ResoniteModLoader;

namespace DirectAttachments
{
    public class DirectAttachments : ResoniteMod
    {
        public override String Name => "DirectAttachments";
        public override String Author => "zahndy";
        public override String Link => "https://github.com/zahndy/DirectAttachments";
        public override String Version => "1.0.0";

        public static ModConfiguration config;

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool> ENABLED = new ModConfigurationKey<bool>("enabled", "Enabled", () => true);

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<dummy> DUMMY_ = new ModConfigurationKey<dummy>("dummy_", $"<size=300>Enter stuff you want to attach. (Inventory path works best eg resrec:///U-user/Inventory/SpawnObjects/Object. \n that way for edits you only have to update the item, not the resrec)</size> ");

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<string> ContextItem = new ModConfigurationKey<string>("ContextItem", "Context Menu Attachment ", () => "");

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<string> RootItem = new ModConfigurationKey<string>("RootItem", "Avatar Root Attachment ", () => "");

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<string> HeadItem = new ModConfigurationKey<string>("HeadItem", "Head Attachment", () => "");

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<string> RArmItem = new ModConfigurationKey<string>("RArmItem", "Right Arm Attachment", () => "");

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<string> LArmItem = new ModConfigurationKey<string>("LArmItem", "Left Arm Attachment", () => "");

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<string> RHandItem = new ModConfigurationKey<string>("RHandItem", "Right Hand Attachment", () => "");

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<string> LHandItem = new ModConfigurationKey<string>("LHandItem", "Left Hand Attachment", () => "");
        public override void OnEngineInit()
        {
            config = GetConfiguration()!;
            config.Save(true);
            Harmony harmony = new Harmony("com.zahndy.DirectAttachments");
            harmony.PatchAll();
        }
    
        [HarmonyPatch(typeof(CharacterController), "OnAttach")]
        class CharacterControllerOnAttachPatch
        {
            public static void Postfix(CharacterController __instance)
            {
                if (__instance.Slot.ActiveUser != __instance.LocalUser) { return; }
                //else { Msg("ActiveUser not " + __instance.LocalUser + " but: " + __instance.Slot.ActiveUser.ToString()); }
                if (DirectAttachments.config.GetValue(DirectAttachments.ENABLED) == true)
                {
                    Msg("enabled");
                    __instance.RunSynchronously(() =>
                    {
                        User user = __instance.Slot.ActiveUser;
                        if (user?.UserName == null || !user.IsLocalUser) return;

                        AvatarObjectSlot comp = __instance.Slot.GetComponent<AvatarObjectSlot>();
                        if (comp != null)
                        {
                            comp.Equipped.Changed += (slot) =>
                            {
                                AddRightArmAttachment(user);
                            };
                        }

                        string contextitemPath = DirectAttachments.config.GetValue(DirectAttachments.ContextItem)!.Trim(',', ' ');
                        if (contextitemPath.Length > 1)
                        {
                            Slot radial = user.LocalUserRoot.Slot.FindChild("Radial Menu", true, true, 2);
                            if (radial != null)
                            {
                                Uri SlotAttachment = new Uri(contextitemPath);
                                Slot slot = radial.AddSlot("Holder");
                                slot.LoadObjectAsync(SlotAttachment);
                                Msg("Attached item to " + slot.Name);
                            }
                            else
                            {
                                Msg("Radial Menu not found");
                            }
                        }

                        string rootitemPath = DirectAttachments.config.GetValue(DirectAttachments.RootItem)!.Trim(',', ' ');
                        if (rootitemPath.Length > 1)
                        {
                            Slot root = user.Root.Slot;
                            if (root != null)
                            {
                                Uri SlotAttachment = new Uri(rootitemPath);
                                Slot slot = root.AddSlot("Holder");
                                slot.LoadObjectAsync(SlotAttachment);
                                Msg("Attached root item to " + slot.Name);
                            }
                            else
                            {
                                Msg("User Root not found");
                            }
                        }

                    });
                }
            }
        }

        static void AddRightArmAttachment(User user)
        {
            if (DirectAttachments.config.GetValue(DirectAttachments.ENABLED) == true)
            {
                if (user?.UserName == null || !user.IsLocalUser) return;

                string rarmitemPath = DirectAttachments.config.GetValue(DirectAttachments.RArmItem)!.Trim(',', ' ');
                if (rarmitemPath.Length > 1)
                {
                    Slot elbow = user.GetBodyNodeSlot(Renderite.Shared.BodyNode.RightLowerArm);
                    if (elbow != null)
                    {
                        elbow = elbow.AddSlot("Holder");
                        Uri SlotAttachment = new Uri(rarmitemPath);
                        elbow.LoadObjectAsync(SlotAttachment);
                        Msg("attached elbow object");
                    }
                    else
                    {
                        Msg("Right elbow not found");
                    }
                }
                string larmitemPath = DirectAttachments.config.GetValue(DirectAttachments.LArmItem)!.Trim(',', ' ');
                if (larmitemPath.Length > 1)
                {
                    Slot elbow = user.GetBodyNodeSlot(Renderite.Shared.BodyNode.LeftLowerArm);
                    if (elbow != null)
                    {
                        elbow = elbow.AddSlot("Holder");
                        Uri SlotAttachment = new Uri(larmitemPath);
                        elbow.LoadObjectAsync(SlotAttachment);
                        Msg("attached elbow object");
                    }
                    else
                    {
                        Msg("Left elbow not found");
                    }
                }
                string headitemPath = DirectAttachments.config.GetValue(DirectAttachments.HeadItem)!.Trim(',', ' ');
                if (headitemPath.Length > 1)
                {
                    Slot head = user.GetBodyNodeSlot(Renderite.Shared.BodyNode.Head);
                    if (head != null)
                    {
                        head = head.AddSlot("Holder");
                        Uri SlotAttachment = new Uri(headitemPath);
                        head.LoadObjectAsync(SlotAttachment);
                        Msg("attached head object");
                    }
                    else
                    {
                        Msg("Head not found");
                    }
                }
                string rhanditemPath = DirectAttachments.config.GetValue(DirectAttachments.RHandItem)!.Trim(',', ' ');
                if (rhanditemPath.Length > 1)
                {
                    Slot hand = user.GetBodyNodeSlot(Renderite.Shared.BodyNode.RightHand);
                    if (hand != null)
                    {
                        hand = hand.AddSlot("Holder");
                        Uri SlotAttachment = new Uri(rhanditemPath);
                        hand.LoadObjectAsync(SlotAttachment);
                        Msg("attached right hand object");
                    }
                    else
                    {
                        Msg("Right hand not found");
                    }
                }
                string lhanditemPath = DirectAttachments.config.GetValue(DirectAttachments.LHandItem)!.Trim(',', ' ');
                if (lhanditemPath.Length > 1)
                {
                    Slot hand = user.GetBodyNodeSlot(Renderite.Shared.BodyNode.LeftHand);
                    if (hand != null)
                    {
                        hand = hand.AddSlot("Holder");
                        Uri SlotAttachment = new Uri(lhanditemPath);
                        hand.LoadObjectAsync(SlotAttachment);
                        Msg("attached left hand object");
                    }
                    else
                    {
                        Msg("Left hand not found");
                    }
                }
            }
        }
    }
}

