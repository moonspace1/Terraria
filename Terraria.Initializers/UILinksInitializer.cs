using System;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;
using Terraria.Social;
using Terraria.UI;
using Terraria.UI.Gamepad;

namespace Terraria.Initializers;

public class UILinksInitializer
{
	public class SomeVarsForUILinkers
	{
		public static Recipe SequencedCraftingCurrent;

		public static int HairMoveCD;
	}

	public static bool NothingMoreImportantThanNPCChat()
	{
		if (!Main.hairWindow && Main.npcShop == 0)
		{
			return Main.player[Main.myPlayer].chest == -1;
		}
		return false;
	}

	public static float HandleSliderHorizontalInput(float currentValue, float min, float max, float deadZone = 0.2f, float sensitivity = 0.5f)
	{
		float x = PlayerInput.GamepadThumbstickLeft.X;
		x = ((!(x < 0f - deadZone) && !(x > deadZone)) ? 0f : (MathHelper.Lerp(0f, sensitivity / 60f, (Math.Abs(x) - deadZone) / (1f - deadZone)) * (float)Math.Sign(x)));
		return MathHelper.Clamp((currentValue - min) / (max - min) + x, 0f, 1f) * (max - min) + min;
	}

	public static float HandleSliderVerticalInput(float currentValue, float min, float max, float deadZone = 0.2f, float sensitivity = 0.5f)
	{
		float num = 0f - PlayerInput.GamepadThumbstickLeft.Y;
		num = ((!(num < 0f - deadZone) && !(num > deadZone)) ? 0f : (MathHelper.Lerp(0f, sensitivity / 60f, (Math.Abs(num) - deadZone) / (1f - deadZone)) * (float)Math.Sign(num)));
		return MathHelper.Clamp((currentValue - min) / (max - min) + num, 0f, 1f) * (max - min) + min;
	}

	public static bool CanExecuteInputCommand()
	{
		return PlayerInput.AllowExecutionOfGamepadInstructions;
	}

	public static void Load()
	{
		Func<string> value = () => PlayerInput.BuildCommand(Lang.misc[53].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
		UILinkPage uILinkPage = new UILinkPage();
		uILinkPage.UpdateEvent += delegate
		{
			PlayerInput.GamepadAllowScrolling = true;
		};
		for (int i = 0; i < 20; i++)
		{
			uILinkPage.LinkMap.Add(2000 + i, new UILinkPoint(2000 + i, enabled: true, -3, -4, -1, -2));
		}
		uILinkPage.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[53].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]) + PlayerInput.BuildCommand(Lang.misc[82].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]);
		uILinkPage.UpdateEvent += delegate
		{
			bool flag = PlayerInput.Triggers.JustPressed.Inventory;
			if (Main.inputTextEscape)
			{
				Main.inputTextEscape = false;
				flag = true;
			}
			if (CanExecuteInputCommand() && flag)
			{
				FancyExit();
			}
			UILinkPointNavigator.Shortcuts.BackButtonInUse = flag;
			HandleOptionsSpecials();
		};
		uILinkPage.IsValidEvent += () => Main.gameMenu && !Main.MenuUI.IsVisible;
		uILinkPage.CanEnterEvent += () => Main.gameMenu && !Main.MenuUI.IsVisible;
		UILinkPointNavigator.RegisterPage(uILinkPage, 1000);
		UILinkPage cp = new UILinkPage();
		cp.LinkMap.Add(2500, new UILinkPoint(2500, enabled: true, -3, 2501, -1, -2));
		cp.LinkMap.Add(2501, new UILinkPoint(2501, enabled: true, 2500, 2502, -1, -2));
		cp.LinkMap.Add(2502, new UILinkPoint(2502, enabled: true, 2501, 2503, -1, -2));
		cp.LinkMap.Add(2503, new UILinkPoint(2503, enabled: true, 2502, -4, -1, -2));
		cp.UpdateEvent += delegate
		{
			cp.LinkMap[2501].Right = (UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight ? 2502 : (-4));
			if (cp.LinkMap[2501].Right == -4 && UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight2)
			{
				cp.LinkMap[2501].Right = 2503;
			}
			cp.LinkMap[2502].Right = (UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight2 ? 2503 : (-4));
			cp.LinkMap[2503].Left = (UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight ? 2502 : 2501);
		};
		cp.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[53].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]) + PlayerInput.BuildCommand(Lang.misc[56].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]);
		cp.IsValidEvent += () => (Main.player[Main.myPlayer].talkNPC != -1 || Main.player[Main.myPlayer].sign != -1) && NothingMoreImportantThanNPCChat();
		cp.CanEnterEvent += () => (Main.player[Main.myPlayer].talkNPC != -1 || Main.player[Main.myPlayer].sign != -1) && NothingMoreImportantThanNPCChat();
		cp.EnterEvent += delegate
		{
			Main.player[Main.myPlayer].releaseInventory = false;
		};
		cp.LeaveEvent += delegate
		{
			Main.npcChatRelease = false;
			Main.player[Main.myPlayer].LockGamepadTileInteractions();
		};
		UILinkPointNavigator.RegisterPage(cp, 1003);
		UILinkPage cp2 = new UILinkPage();
		cp2.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value2 = delegate
		{
			int currentPoint = UILinkPointNavigator.CurrentPoint;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].inventory, 0, currentPoint);
		};
		Func<string> value3 = () => ItemSlot.GetGamepadInstructions(ref Main.player[Main.myPlayer].trashItem, 6);
		for (int j = 0; j <= 49; j++)
		{
			UILinkPoint uILinkPoint = new UILinkPoint(j, enabled: true, j - 1, j + 1, j - 10, j + 10);
			uILinkPoint.OnSpecialInteracts += value2;
			int num = j;
			if (num < 10)
			{
				uILinkPoint.Up = -1;
			}
			if (num >= 40)
			{
				uILinkPoint.Down = -2;
			}
			if (num % 10 == 9)
			{
				uILinkPoint.Right = -4;
			}
			if (num % 10 == 0)
			{
				uILinkPoint.Left = -3;
			}
			cp2.LinkMap.Add(j, uILinkPoint);
		}
		cp2.LinkMap[9].Right = 0;
		cp2.LinkMap[19].Right = 50;
		cp2.LinkMap[29].Right = 51;
		cp2.LinkMap[39].Right = 52;
		cp2.LinkMap[49].Right = 53;
		cp2.LinkMap[0].Left = 9;
		cp2.LinkMap[10].Left = 54;
		cp2.LinkMap[20].Left = 55;
		cp2.LinkMap[30].Left = 56;
		cp2.LinkMap[40].Left = 57;
		cp2.LinkMap.Add(300, new UILinkPoint(300, enabled: true, 309, 310, 49, -2));
		cp2.LinkMap.Add(309, new UILinkPoint(309, enabled: true, 310, 300, 302, 54));
		cp2.LinkMap.Add(310, new UILinkPoint(310, enabled: true, 300, 309, 301, 50));
		cp2.LinkMap.Add(301, new UILinkPoint(301, enabled: true, 300, 302, 53, 50));
		cp2.LinkMap.Add(302, new UILinkPoint(302, enabled: true, 301, 310, 57, 54));
		cp2.LinkMap.Add(311, new UILinkPoint(311, enabled: true, -3, -4, 40, -2));
		cp2.LinkMap[301].OnSpecialInteracts += value;
		cp2.LinkMap[302].OnSpecialInteracts += value;
		cp2.LinkMap[309].OnSpecialInteracts += value;
		cp2.LinkMap[310].OnSpecialInteracts += value;
		cp2.LinkMap[300].OnSpecialInteracts += value3;
		cp2.UpdateEvent += delegate
		{
			bool inReforgeMenu = Main.InReforgeMenu;
			bool flag2 = Main.player[Main.myPlayer].chest != -1;
			bool flag3 = Main.npcShop != 0;
			TileEntity tileEntity = Main.LocalPlayer.tileEntityAnchor.GetTileEntity();
			bool flag4 = tileEntity is TEHatRack;
			bool flag5 = tileEntity is TEDisplayDoll;
			for (int k = 40; k <= 49; k++)
			{
				if (inReforgeMenu)
				{
					cp2.LinkMap[k].Down = ((k < 45) ? 303 : 304);
				}
				else if (flag2)
				{
					cp2.LinkMap[k].Down = 400 + k - 40;
				}
				else if (flag3)
				{
					cp2.LinkMap[k].Down = 2700 + k - 40;
				}
				else if (k == 40)
				{
					cp2.LinkMap[k].Down = 311;
				}
				else
				{
					cp2.LinkMap[k].Down = -2;
				}
			}
			if (flag5)
			{
				for (int l = 40; l <= 47; l++)
				{
					cp2.LinkMap[l].Down = 5100 + l - 40;
				}
			}
			if (flag4)
			{
				for (int m = 44; m <= 45; m++)
				{
					cp2.LinkMap[m].Down = 5000 + m - 44;
				}
			}
			if (flag2)
			{
				cp2.LinkMap[300].Up = 439;
				cp2.LinkMap[300].Right = -4;
				cp2.LinkMap[300].Left = 309;
				cp2.LinkMap[309].Up = 438;
				cp2.LinkMap[309].Right = 300;
				cp2.LinkMap[309].Left = 310;
				cp2.LinkMap[310].Up = 437;
				cp2.LinkMap[310].Right = 309;
				cp2.LinkMap[310].Left = -3;
			}
			else if (flag3)
			{
				cp2.LinkMap[300].Up = 2739;
				cp2.LinkMap[300].Right = -4;
				cp2.LinkMap[300].Left = 309;
				cp2.LinkMap[309].Up = 2738;
				cp2.LinkMap[309].Right = 300;
				cp2.LinkMap[309].Left = 310;
				cp2.LinkMap[310].Up = 2737;
				cp2.LinkMap[310].Right = 309;
				cp2.LinkMap[310].Left = -3;
			}
			else
			{
				cp2.LinkMap[49].Down = 300;
				cp2.LinkMap[48].Down = 309;
				cp2.LinkMap[47].Down = 310;
				cp2.LinkMap[300].Up = 49;
				cp2.LinkMap[300].Right = 301;
				cp2.LinkMap[300].Left = 309;
				cp2.LinkMap[309].Up = 48;
				cp2.LinkMap[309].Right = 300;
				cp2.LinkMap[309].Left = 310;
				cp2.LinkMap[310].Up = 47;
				cp2.LinkMap[310].Right = 309;
				cp2.LinkMap[310].Left = 302;
			}
			cp2.LinkMap[0].Left = 9;
			cp2.LinkMap[10].Left = 54;
			cp2.LinkMap[20].Left = 55;
			cp2.LinkMap[30].Left = 56;
			cp2.LinkMap[40].Left = 57;
			if (UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 0)
			{
				cp2.LinkMap[0].Left = 6000;
			}
			if (UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 2)
			{
				cp2.LinkMap[10].Left = 6002;
			}
			if (UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 4)
			{
				cp2.LinkMap[20].Left = 6004;
			}
			if (UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 6)
			{
				cp2.LinkMap[30].Left = 6006;
			}
			if (UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 8)
			{
				cp2.LinkMap[40].Left = 6008;
			}
			cp2.PageOnLeft = 9;
			if (Main.CreativeMenu.Enabled)
			{
				cp2.PageOnLeft = 1005;
			}
			if (Main.InReforgeMenu)
			{
				cp2.PageOnLeft = 5;
			}
		};
		cp2.IsValidEvent += () => Main.playerInventory;
		cp2.PageOnLeft = 9;
		cp2.PageOnRight = 2;
		UILinkPointNavigator.RegisterPage(cp2, 0);
		UILinkPage cp3 = new UILinkPage();
		cp3.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value4 = delegate
		{
			int currentPoint2 = UILinkPointNavigator.CurrentPoint;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].inventory, 1, currentPoint2);
		};
		for (int n = 50; n <= 53; n++)
		{
			UILinkPoint uILinkPoint2 = new UILinkPoint(n, enabled: true, -3, -4, n - 1, n + 1);
			uILinkPoint2.OnSpecialInteracts += value4;
			cp3.LinkMap.Add(n, uILinkPoint2);
		}
		cp3.LinkMap[50].Left = 19;
		cp3.LinkMap[51].Left = 29;
		cp3.LinkMap[52].Left = 39;
		cp3.LinkMap[53].Left = 49;
		cp3.LinkMap[50].Right = 54;
		cp3.LinkMap[51].Right = 55;
		cp3.LinkMap[52].Right = 56;
		cp3.LinkMap[53].Right = 57;
		cp3.LinkMap[50].Up = -1;
		cp3.LinkMap[53].Down = -2;
		cp3.UpdateEvent += delegate
		{
			if (Main.player[Main.myPlayer].chest == -1 && Main.npcShop == 0)
			{
				cp3.LinkMap[50].Up = 301;
				cp3.LinkMap[53].Down = 301;
			}
			else
			{
				cp3.LinkMap[50].Up = 504;
				cp3.LinkMap[53].Down = 500;
			}
		};
		cp3.IsValidEvent += () => Main.playerInventory;
		cp3.PageOnLeft = 0;
		cp3.PageOnRight = 2;
		UILinkPointNavigator.RegisterPage(cp3, 1);
		UILinkPage cp4 = new UILinkPage();
		cp4.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value5 = delegate
		{
			int currentPoint3 = UILinkPointNavigator.CurrentPoint;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].inventory, 2, currentPoint3);
		};
		for (int num2 = 54; num2 <= 57; num2++)
		{
			UILinkPoint uILinkPoint3 = new UILinkPoint(num2, enabled: true, -3, -4, num2 - 1, num2 + 1);
			uILinkPoint3.OnSpecialInteracts += value5;
			cp4.LinkMap.Add(num2, uILinkPoint3);
		}
		cp4.LinkMap[54].Left = 50;
		cp4.LinkMap[55].Left = 51;
		cp4.LinkMap[56].Left = 52;
		cp4.LinkMap[57].Left = 53;
		cp4.LinkMap[54].Right = 10;
		cp4.LinkMap[55].Right = 20;
		cp4.LinkMap[56].Right = 30;
		cp4.LinkMap[57].Right = 40;
		cp4.LinkMap[54].Up = -1;
		cp4.LinkMap[57].Down = -2;
		cp4.UpdateEvent += delegate
		{
			if (Main.player[Main.myPlayer].chest == -1 && Main.npcShop == 0)
			{
				cp4.LinkMap[54].Up = 302;
				cp4.LinkMap[57].Down = 302;
			}
			else
			{
				cp4.LinkMap[54].Up = 504;
				cp4.LinkMap[57].Down = 500;
			}
		};
		cp4.PageOnLeft = 0;
		cp4.PageOnRight = 8;
		UILinkPointNavigator.RegisterPage(cp4, 2);
		UILinkPage cp5 = new UILinkPage();
		cp5.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value6 = delegate
		{
			int num3 = UILinkPointNavigator.CurrentPoint - 100;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].armor, (num3 < 10) ? 8 : 9, num3);
		};
		Func<string> value7 = delegate
		{
			int slot = UILinkPointNavigator.CurrentPoint - 120;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].dye, 12, slot);
		};
		for (int num4 = 100; num4 <= 119; num4++)
		{
			UILinkPoint uILinkPoint4 = new UILinkPoint(num4, enabled: true, num4 + 10, num4 - 10, num4 - 1, num4 + 1);
			uILinkPoint4.OnSpecialInteracts += value6;
			int num5 = num4 - 100;
			if (num5 == 0)
			{
				uILinkPoint4.Up = 305;
			}
			if (num5 == 10)
			{
				uILinkPoint4.Up = 306;
			}
			if (num5 == 9 || num5 == 19)
			{
				uILinkPoint4.Down = -2;
			}
			if (num5 >= 10)
			{
				uILinkPoint4.Left = 120 + num5 % 10;
			}
			else if (num5 >= 3)
			{
				uILinkPoint4.Right = -4;
			}
			else
			{
				uILinkPoint4.Right = 312 + num5;
			}
			cp5.LinkMap.Add(num4, uILinkPoint4);
		}
		for (int num6 = 120; num6 <= 129; num6++)
		{
			UILinkPoint uILinkPoint4 = new UILinkPoint(num6, enabled: true, -3, -10 + num6, num6 - 1, num6 + 1);
			uILinkPoint4.OnSpecialInteracts += value7;
			int num7 = num6 - 120;
			if (num7 == 0)
			{
				uILinkPoint4.Up = 307;
			}
			if (num7 == 9)
			{
				uILinkPoint4.Down = 308;
				uILinkPoint4.Left = 1557;
			}
			if (num7 == 8)
			{
				uILinkPoint4.Left = 1570;
			}
			cp5.LinkMap.Add(num6, uILinkPoint4);
		}
		for (int num8 = 312; num8 <= 314; num8++)
		{
			int num9 = num8 - 312;
			UILinkPoint uILinkPoint4 = new UILinkPoint(num8, enabled: true, 100 + num9, -4, num8 - 1, num8 + 1);
			if (num9 == 0)
			{
				uILinkPoint4.Up = -1;
			}
			if (num9 == 2)
			{
				uILinkPoint4.Down = -2;
			}
			uILinkPoint4.OnSpecialInteracts += value;
			cp5.LinkMap.Add(num8, uILinkPoint4);
		}
		cp5.IsValidEvent += () => Main.playerInventory && Main.EquipPage == 0;
		cp5.UpdateEvent += delegate
		{
			int num10 = 107;
			int amountOfExtraAccessorySlotsToShow = Main.player[Main.myPlayer].GetAmountOfExtraAccessorySlotsToShow();
			for (int num11 = 0; num11 < amountOfExtraAccessorySlotsToShow; num11++)
			{
				cp5.LinkMap[num10 + num11].Down = num10 + num11 + 1;
				cp5.LinkMap[num10 - 100 + 120 + num11].Down = num10 - 100 + 120 + num11 + 1;
				cp5.LinkMap[num10 + 10 + num11].Down = num10 + 10 + num11 + 1;
			}
			cp5.LinkMap[num10 + amountOfExtraAccessorySlotsToShow].Down = 308;
			cp5.LinkMap[num10 + 10 + amountOfExtraAccessorySlotsToShow].Down = 308;
			cp5.LinkMap[num10 - 100 + 120 + amountOfExtraAccessorySlotsToShow].Down = 308;
			bool shouldPVPDraw = Main.ShouldPVPDraw;
			for (int num12 = 120; num12 <= 129; num12++)
			{
				UILinkPoint uILinkPoint5 = cp5.LinkMap[num12];
				int num13 = num12 - 120;
				uILinkPoint5.Left = -3;
				if (num13 == 0)
				{
					uILinkPoint5.Left = (shouldPVPDraw ? 1550 : (-3));
				}
				if (num13 == 1)
				{
					uILinkPoint5.Left = (shouldPVPDraw ? 1552 : (-3));
				}
				if (num13 == 2)
				{
					uILinkPoint5.Left = (shouldPVPDraw ? 1556 : (-3));
				}
				if (num13 == 3)
				{
					uILinkPoint5.Left = ((UILinkPointNavigator.Shortcuts.INFOACCCOUNT >= 1) ? 1558 : (-3));
				}
				if (num13 == 4)
				{
					uILinkPoint5.Left = ((UILinkPointNavigator.Shortcuts.INFOACCCOUNT >= 5) ? 1562 : (-3));
				}
				if (num13 == 5)
				{
					uILinkPoint5.Left = ((UILinkPointNavigator.Shortcuts.INFOACCCOUNT >= 9) ? 1566 : (-3));
				}
			}
			cp5.LinkMap[num10 - 100 + 120 + amountOfExtraAccessorySlotsToShow].Left = 1557;
			cp5.LinkMap[num10 - 100 + 120 + amountOfExtraAccessorySlotsToShow - 1].Left = 1570;
		};
		cp5.PageOnLeft = 8;
		cp5.PageOnRight = 8;
		UILinkPointNavigator.RegisterPage(cp5, 3);
		UILinkPage uILinkPage2 = new UILinkPage();
		uILinkPage2.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value8 = delegate
		{
			int slot2 = UILinkPointNavigator.CurrentPoint - 400;
			int context = 4;
			Item[] item = Main.player[Main.myPlayer].bank.item;
			switch (Main.player[Main.myPlayer].chest)
			{
			case -1:
				return "";
			case -3:
				item = Main.player[Main.myPlayer].bank2.item;
				break;
			case -4:
				item = Main.player[Main.myPlayer].bank3.item;
				break;
			case -5:
				item = Main.player[Main.myPlayer].bank4.item;
				context = 32;
				break;
			default:
				item = Main.chest[Main.player[Main.myPlayer].chest].item;
				context = 3;
				break;
			case -2:
				break;
			}
			return ItemSlot.GetGamepadInstructions(item, context, slot2);
		};
		for (int num14 = 400; num14 <= 439; num14++)
		{
			UILinkPoint uILinkPoint6 = new UILinkPoint(num14, enabled: true, num14 - 1, num14 + 1, num14 - 10, num14 + 10);
			uILinkPoint6.OnSpecialInteracts += value8;
			int num15 = num14 - 400;
			if (num15 < 10)
			{
				uILinkPoint6.Up = 40 + num15;
			}
			if (num15 >= 30)
			{
				uILinkPoint6.Down = -2;
			}
			if (num15 % 10 == 9)
			{
				uILinkPoint6.Right = -4;
			}
			if (num15 % 10 == 0)
			{
				uILinkPoint6.Left = -3;
			}
			uILinkPage2.LinkMap.Add(num14, uILinkPoint6);
		}
		uILinkPage2.LinkMap.Add(500, new UILinkPoint(500, enabled: true, 409, -4, 53, 501));
		uILinkPage2.LinkMap.Add(501, new UILinkPoint(501, enabled: true, 419, -4, 500, 502));
		uILinkPage2.LinkMap.Add(502, new UILinkPoint(502, enabled: true, 429, -4, 501, 503));
		uILinkPage2.LinkMap.Add(503, new UILinkPoint(503, enabled: true, 439, -4, 502, 505));
		uILinkPage2.LinkMap.Add(505, new UILinkPoint(505, enabled: true, 439, -4, 503, 504));
		uILinkPage2.LinkMap.Add(504, new UILinkPoint(504, enabled: true, 439, -4, 505, 50));
		uILinkPage2.LinkMap.Add(506, new UILinkPoint(506, enabled: true, 439, -4, 505, 50));
		uILinkPage2.LinkMap[500].OnSpecialInteracts += value;
		uILinkPage2.LinkMap[501].OnSpecialInteracts += value;
		uILinkPage2.LinkMap[502].OnSpecialInteracts += value;
		uILinkPage2.LinkMap[503].OnSpecialInteracts += value;
		uILinkPage2.LinkMap[504].OnSpecialInteracts += value;
		uILinkPage2.LinkMap[505].OnSpecialInteracts += value;
		uILinkPage2.LinkMap[506].OnSpecialInteracts += value;
		uILinkPage2.LinkMap[409].Right = 500;
		uILinkPage2.LinkMap[419].Right = 501;
		uILinkPage2.LinkMap[429].Right = 502;
		uILinkPage2.LinkMap[439].Right = 503;
		uILinkPage2.LinkMap[439].Down = 300;
		uILinkPage2.LinkMap[438].Down = 309;
		uILinkPage2.LinkMap[437].Down = 310;
		uILinkPage2.PageOnLeft = 0;
		uILinkPage2.PageOnRight = 0;
		uILinkPage2.DefaultPoint = 400;
		UILinkPointNavigator.RegisterPage(uILinkPage2, 4, automatedDefault: false);
		uILinkPage2.IsValidEvent += () => Main.playerInventory && Main.player[Main.myPlayer].chest != -1;
		UILinkPage uILinkPage3 = new UILinkPage();
		uILinkPage3.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value9 = delegate
		{
			int slot3 = UILinkPointNavigator.CurrentPoint - 5100;
			return (!(Main.LocalPlayer.tileEntityAnchor.GetTileEntity() is TEDisplayDoll tEDisplayDoll)) ? "" : tEDisplayDoll.GetItemGamepadInstructions(slot3);
		};
		for (int num16 = 5100; num16 <= 5115; num16++)
		{
			UILinkPoint uILinkPoint7 = new UILinkPoint(num16, enabled: true, num16 - 1, num16 + 1, num16 - 8, num16 + 8);
			uILinkPoint7.OnSpecialInteracts += value9;
			int num17 = num16 - 5100;
			if (num17 < 8)
			{
				uILinkPoint7.Up = 40 + num17;
			}
			if (num17 >= 8)
			{
				uILinkPoint7.Down = -2;
			}
			if (num17 % 8 == 7)
			{
				uILinkPoint7.Right = -4;
			}
			if (num17 % 8 == 0)
			{
				uILinkPoint7.Left = -3;
			}
			uILinkPage3.LinkMap.Add(num16, uILinkPoint7);
		}
		uILinkPage3.PageOnLeft = 0;
		uILinkPage3.PageOnRight = 0;
		uILinkPage3.DefaultPoint = 5100;
		UILinkPointNavigator.RegisterPage(uILinkPage3, 20, automatedDefault: false);
		uILinkPage3.IsValidEvent += () => Main.playerInventory && Main.LocalPlayer.tileEntityAnchor.GetTileEntity() is TEDisplayDoll;
		UILinkPage uILinkPage4 = new UILinkPage();
		uILinkPage4.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value10 = delegate
		{
			int slot4 = UILinkPointNavigator.CurrentPoint - 5000;
			return (!(Main.LocalPlayer.tileEntityAnchor.GetTileEntity() is TEHatRack tEHatRack)) ? "" : tEHatRack.GetItemGamepadInstructions(slot4);
		};
		for (int num18 = 5000; num18 <= 5003; num18++)
		{
			UILinkPoint uILinkPoint8 = new UILinkPoint(num18, enabled: true, num18 - 1, num18 + 1, num18 - 2, num18 + 2);
			uILinkPoint8.OnSpecialInteracts += value10;
			int num19 = num18 - 5000;
			if (num19 < 2)
			{
				uILinkPoint8.Up = 44 + num19;
			}
			if (num19 >= 2)
			{
				uILinkPoint8.Down = -2;
			}
			if (num19 % 2 == 1)
			{
				uILinkPoint8.Right = -4;
			}
			if (num19 % 2 == 0)
			{
				uILinkPoint8.Left = -3;
			}
			uILinkPage4.LinkMap.Add(num18, uILinkPoint8);
		}
		uILinkPage4.PageOnLeft = 0;
		uILinkPage4.PageOnRight = 0;
		uILinkPage4.DefaultPoint = 5000;
		UILinkPointNavigator.RegisterPage(uILinkPage4, 21, automatedDefault: false);
		uILinkPage4.IsValidEvent += () => Main.playerInventory && Main.LocalPlayer.tileEntityAnchor.GetTileEntity() is TEHatRack;
		UILinkPage uILinkPage5 = new UILinkPage();
		uILinkPage5.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value11 = delegate
		{
			if (Main.npcShop == 0)
			{
				return "";
			}
			int slot5 = UILinkPointNavigator.CurrentPoint - 2700;
			return ItemSlot.GetGamepadInstructions(Main.instance.shop[Main.npcShop].item, 15, slot5);
		};
		for (int num20 = 2700; num20 <= 2739; num20++)
		{
			UILinkPoint uILinkPoint9 = new UILinkPoint(num20, enabled: true, num20 - 1, num20 + 1, num20 - 10, num20 + 10);
			uILinkPoint9.OnSpecialInteracts += value11;
			int num21 = num20 - 2700;
			if (num21 < 10)
			{
				uILinkPoint9.Up = 40 + num21;
			}
			if (num21 >= 30)
			{
				uILinkPoint9.Down = -2;
			}
			if (num21 % 10 == 9)
			{
				uILinkPoint9.Right = -4;
			}
			if (num21 % 10 == 0)
			{
				uILinkPoint9.Left = -3;
			}
			uILinkPage5.LinkMap.Add(num20, uILinkPoint9);
		}
		uILinkPage5.LinkMap[2739].Down = 300;
		uILinkPage5.LinkMap[2738].Down = 309;
		uILinkPage5.LinkMap[2737].Down = 310;
		uILinkPage5.PageOnLeft = 0;
		uILinkPage5.PageOnRight = 0;
		UILinkPointNavigator.RegisterPage(uILinkPage5, 13);
		uILinkPage5.IsValidEvent += () => Main.playerInventory && Main.npcShop != 0;
		UILinkPage cp6 = new UILinkPage();
		cp6.LinkMap.Add(303, new UILinkPoint(303, enabled: true, 304, 304, 40, -2));
		cp6.LinkMap.Add(304, new UILinkPoint(304, enabled: true, 303, 303, 40, -2));
		cp6.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value12 = () => ItemSlot.GetGamepadInstructions(ref Main.reforgeItem, 5);
		cp6.LinkMap[303].OnSpecialInteracts += value12;
		cp6.LinkMap[304].OnSpecialInteracts += () => Lang.misc[53].Value;
		cp6.UpdateEvent += delegate
		{
			if (Main.reforgeItem.type > 0)
			{
				cp6.LinkMap[303].Left = (cp6.LinkMap[303].Right = 304);
			}
			else
			{
				if (UILinkPointNavigator.OverridePoint == -1 && cp6.CurrentPoint == 304)
				{
					UILinkPointNavigator.ChangePoint(303);
				}
				cp6.LinkMap[303].Left = -3;
				cp6.LinkMap[303].Right = -4;
			}
		};
		cp6.IsValidEvent += () => Main.playerInventory && Main.InReforgeMenu;
		cp6.PageOnLeft = 0;
		cp6.PageOnRight = 0;
		cp6.EnterEvent += delegate
		{
			PlayerInput.LockGamepadButtons("MouseLeft");
		};
		UILinkPointNavigator.RegisterPage(cp6, 5);
		UILinkPage cp7 = new UILinkPage();
		cp7.OnSpecialInteracts += delegate
		{
			bool flag6 = UILinkPointNavigator.CurrentPoint == 600;
			bool flag7 = !flag6 && WorldGen.IsNPCEvictable(UILinkPointNavigator.Shortcuts.NPCS_LastHovered);
			if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.Grapple)
			{
				Point point = Main.player[Main.myPlayer].Center.ToTileCoordinates();
				if (flag6)
				{
					if (WorldGen.MoveTownNPC(point.X, point.Y, -1))
					{
						Main.NewText(Lang.inter[39].Value, byte.MaxValue, 240, 20);
					}
					SoundEngine.PlaySound(12);
				}
				else if (WorldGen.MoveTownNPC(point.X, point.Y, UILinkPointNavigator.Shortcuts.NPCS_LastHovered))
				{
					WorldGen.moveRoom(point.X, point.Y, UILinkPointNavigator.Shortcuts.NPCS_LastHovered);
					SoundEngine.PlaySound(12);
				}
				PlayerInput.LockGamepadButtons("Grapple");
				PlayerInput.SettingsForUI.TryRevertingToMouseMode();
			}
			if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.SmartSelect)
			{
				UILinkPointNavigator.Shortcuts.NPCS_IconsDisplay = !UILinkPointNavigator.Shortcuts.NPCS_IconsDisplay;
				PlayerInput.LockGamepadButtons("SmartSelect");
				PlayerInput.SettingsForUI.TryRevertingToMouseMode();
			}
			if (flag7 && CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.MouseRight)
			{
				WorldGen.kickOut(UILinkPointNavigator.Shortcuts.NPCS_LastHovered);
			}
			return PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]) + PlayerInput.BuildCommand(Lang.misc[70].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]) + PlayerInput.BuildCommand(Lang.misc[69].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["SmartSelect"]) + (flag7 ? PlayerInput.BuildCommand("Evict", false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseRight"]) : "");
		};
		for (int num22 = 600; num22 <= 650; num22++)
		{
			UILinkPoint value13 = new UILinkPoint(num22, enabled: true, num22 + 10, num22 - 10, num22 - 1, num22 + 1);
			cp7.LinkMap.Add(num22, value13);
		}
		cp7.UpdateEvent += delegate
		{
			int num23 = UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn;
			if (num23 == 0)
			{
				num23 = 100;
			}
			for (int num24 = 0; num24 < 50; num24++)
			{
				cp7.LinkMap[600 + num24].Up = ((num24 % num23 == 0) ? (-1) : (600 + num24 - 1));
				if (cp7.LinkMap[600 + num24].Up == -1)
				{
					if (num24 >= num23 * 2)
					{
						cp7.LinkMap[600 + num24].Up = 307;
					}
					else if (num24 >= num23)
					{
						cp7.LinkMap[600 + num24].Up = 306;
					}
					else
					{
						cp7.LinkMap[600 + num24].Up = 305;
					}
				}
				cp7.LinkMap[600 + num24].Down = (((num24 + 1) % num23 == 0 || num24 == UILinkPointNavigator.Shortcuts.NPCS_IconsTotal - 1) ? 308 : (600 + num24 + 1));
				cp7.LinkMap[600 + num24].Left = ((num24 < UILinkPointNavigator.Shortcuts.NPCS_IconsTotal - num23) ? (600 + num24 + num23) : (-3));
				cp7.LinkMap[600 + num24].Right = ((num24 < num23) ? (-4) : (600 + num24 - num23));
			}
		};
		cp7.IsValidEvent += () => Main.playerInventory && Main.EquipPage == 1;
		cp7.PageOnLeft = 8;
		cp7.PageOnRight = 8;
		UILinkPointNavigator.RegisterPage(cp7, 6);
		UILinkPage cp8 = new UILinkPage();
		cp8.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value14 = delegate
		{
			int slot6 = UILinkPointNavigator.CurrentPoint - 180;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscEquips, 20, slot6);
		};
		Func<string> value15 = delegate
		{
			int slot7 = UILinkPointNavigator.CurrentPoint - 180;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscEquips, 19, slot7);
		};
		Func<string> value16 = delegate
		{
			int slot8 = UILinkPointNavigator.CurrentPoint - 180;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscEquips, 18, slot8);
		};
		Func<string> value17 = delegate
		{
			int slot9 = UILinkPointNavigator.CurrentPoint - 180;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscEquips, 17, slot9);
		};
		Func<string> value18 = delegate
		{
			int slot10 = UILinkPointNavigator.CurrentPoint - 180;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscEquips, 16, slot10);
		};
		Func<string> value19 = delegate
		{
			int slot11 = UILinkPointNavigator.CurrentPoint - 185;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscDyes, 33, slot11);
		};
		for (int num25 = 180; num25 <= 184; num25++)
		{
			UILinkPoint uILinkPoint10 = new UILinkPoint(num25, enabled: true, 185 + num25 - 180, -4, num25 - 1, num25 + 1);
			int num26 = num25 - 180;
			if (num26 == 0)
			{
				uILinkPoint10.Up = 305;
			}
			if (num26 == 4)
			{
				uILinkPoint10.Down = 308;
			}
			cp8.LinkMap.Add(num25, uILinkPoint10);
			switch (num25)
			{
			case 180:
				uILinkPoint10.OnSpecialInteracts += value15;
				break;
			case 181:
				uILinkPoint10.OnSpecialInteracts += value14;
				break;
			case 182:
				uILinkPoint10.OnSpecialInteracts += value16;
				break;
			case 183:
				uILinkPoint10.OnSpecialInteracts += value17;
				break;
			case 184:
				uILinkPoint10.OnSpecialInteracts += value18;
				break;
			}
		}
		for (int num27 = 185; num27 <= 189; num27++)
		{
			UILinkPoint uILinkPoint10 = new UILinkPoint(num27, enabled: true, -3, -5 + num27, num27 - 1, num27 + 1);
			uILinkPoint10.OnSpecialInteracts += value19;
			int num28 = num27 - 185;
			if (num28 == 0)
			{
				uILinkPoint10.Up = 306;
			}
			if (num28 == 4)
			{
				uILinkPoint10.Down = 308;
			}
			cp8.LinkMap.Add(num27, uILinkPoint10);
		}
		cp8.UpdateEvent += delegate
		{
			cp8.LinkMap[184].Down = ((UILinkPointNavigator.Shortcuts.BUFFS_DRAWN > 0) ? 9000 : 308);
			cp8.LinkMap[189].Down = ((UILinkPointNavigator.Shortcuts.BUFFS_DRAWN > 0) ? 9000 : 308);
		};
		cp8.IsValidEvent += () => Main.playerInventory && Main.EquipPage == 2;
		cp8.PageOnLeft = 8;
		cp8.PageOnRight = 8;
		UILinkPointNavigator.RegisterPage(cp8, 7);
		UILinkPage cp9 = new UILinkPage();
		cp9.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		cp9.LinkMap.Add(305, new UILinkPoint(305, enabled: true, 306, -4, 308, -2));
		cp9.LinkMap.Add(306, new UILinkPoint(306, enabled: true, 307, 305, 308, -2));
		cp9.LinkMap.Add(307, new UILinkPoint(307, enabled: true, -3, 306, 308, -2));
		cp9.LinkMap.Add(308, new UILinkPoint(308, enabled: true, -3, -4, -1, 305));
		cp9.LinkMap[305].OnSpecialInteracts += value;
		cp9.LinkMap[306].OnSpecialInteracts += value;
		cp9.LinkMap[307].OnSpecialInteracts += value;
		cp9.LinkMap[308].OnSpecialInteracts += value;
		cp9.UpdateEvent += delegate
		{
			switch (Main.EquipPage)
			{
			case 0:
				cp9.LinkMap[305].Down = 100;
				cp9.LinkMap[306].Down = 110;
				cp9.LinkMap[307].Down = 120;
				cp9.LinkMap[308].Up = 108 + Main.player[Main.myPlayer].GetAmountOfExtraAccessorySlotsToShow() - 1;
				break;
			case 1:
			{
				cp9.LinkMap[305].Down = 600;
				cp9.LinkMap[306].Down = ((UILinkPointNavigator.Shortcuts.NPCS_IconsTotal / UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn > 0) ? (600 + UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn) : (-2));
				cp9.LinkMap[307].Down = ((UILinkPointNavigator.Shortcuts.NPCS_IconsTotal / UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn > 1) ? (600 + UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn * 2) : (-2));
				int num29 = UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn;
				if (num29 == 0)
				{
					num29 = 100;
				}
				if (num29 == 100)
				{
					num29 = UILinkPointNavigator.Shortcuts.NPCS_IconsTotal;
				}
				cp9.LinkMap[308].Up = 600 + num29 - 1;
				break;
			}
			case 2:
				cp9.LinkMap[305].Down = 180;
				cp9.LinkMap[306].Down = 185;
				cp9.LinkMap[307].Down = -2;
				cp9.LinkMap[308].Up = ((UILinkPointNavigator.Shortcuts.BUFFS_DRAWN > 0) ? 9000 : 184);
				break;
			}
			cp9.PageOnRight = GetCornerWrapPageIdFromRightToLeft();
		};
		cp9.IsValidEvent += () => Main.playerInventory;
		cp9.PageOnLeft = 0;
		cp9.PageOnRight = 0;
		UILinkPointNavigator.RegisterPage(cp9, 8);
		UILinkPage cp10 = new UILinkPage();
		cp10.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value20 = () => ItemSlot.GetGamepadInstructions(ref Main.guideItem, 7);
		Func<string> HandleItem2 = () => (Main.mouseItem.type < 1) ? "" : ItemSlot.GetGamepadInstructions(ref Main.mouseItem, 22);
		for (int num30 = 1500; num30 < 1550; num30++)
		{
			UILinkPoint uILinkPoint11 = new UILinkPoint(num30, enabled: true, num30, num30, -1, -2);
			if (num30 != 1500)
			{
				uILinkPoint11.OnSpecialInteracts += HandleItem2;
			}
			cp10.LinkMap.Add(num30, uILinkPoint11);
		}
		cp10.LinkMap[1500].OnSpecialInteracts += value20;
		cp10.UpdateEvent += delegate
		{
			int num31 = UILinkPointNavigator.Shortcuts.CRAFT_CurrentIngredientsCount;
			int num32 = num31;
			if (Main.numAvailableRecipes > 0)
			{
				num32 += 2;
			}
			if (num31 < num32)
			{
				num31 = num32;
			}
			if (UILinkPointNavigator.OverridePoint == -1 && cp10.CurrentPoint > 1500 + num31)
			{
				UILinkPointNavigator.ChangePoint(1500);
			}
			if (UILinkPointNavigator.OverridePoint == -1 && cp10.CurrentPoint == 1500 && !Main.InGuideCraftMenu)
			{
				UILinkPointNavigator.ChangePoint(1501);
			}
			for (int num33 = 1; num33 < num31; num33++)
			{
				cp10.LinkMap[1500 + num33].Left = 1500 + num33 - 1;
				cp10.LinkMap[1500 + num33].Right = ((num33 == num31 - 2) ? (-4) : (1500 + num33 + 1));
			}
			cp10.LinkMap[1501].Left = -3;
			if (num31 > 0)
			{
				cp10.LinkMap[1500 + num31 - 1].Right = -4;
			}
			cp10.LinkMap[1500].Down = ((num31 >= 2) ? 1502 : (-2));
			cp10.LinkMap[1500].Left = ((num31 >= 1) ? 1501 : (-3));
			cp10.LinkMap[1502].Up = (Main.InGuideCraftMenu ? 1500 : (-1));
		};
		cp10.LinkMap[1501].OnSpecialInteracts += delegate
		{
			if (Main.InGuideCraftMenu)
			{
				return "";
			}
			string text = "";
			Player player = Main.player[Main.myPlayer];
			bool flag8 = false;
			Item createItem = Main.recipe[Main.availableRecipe[Main.focusRecipe]].createItem;
			if (Main.mouseItem.type == 0 && createItem.maxStack > 1 && player.ItemSpace(createItem).CanTakeItemToPersonalInventory && !player.HasLockedInventory())
			{
				flag8 = true;
				if (CanExecuteInputCommand() && PlayerInput.Triggers.Current.Grapple && Main.stackSplit <= 1)
				{
					if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.Grapple)
					{
						SomeVarsForUILinkers.SequencedCraftingCurrent = Main.recipe[Main.availableRecipe[Main.focusRecipe]];
					}
					ItemSlot.RefreshStackSplitCooldown();
					Main.preventStackSplitReset = true;
					if (SomeVarsForUILinkers.SequencedCraftingCurrent == Main.recipe[Main.availableRecipe[Main.focusRecipe]])
					{
						Main.CraftItem(Main.recipe[Main.availableRecipe[Main.focusRecipe]]);
						Main.mouseItem = player.GetItem(player.whoAmI, Main.mouseItem, new GetItemSettings(LongText: false, NoText: true));
					}
				}
			}
			else if (Main.mouseItem.type > 0 && Main.mouseItem.maxStack == 1 && ItemSlot.Equippable(ref Main.mouseItem))
			{
				text += PlayerInput.BuildCommand(Lang.misc[67].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]);
				if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.Grapple)
				{
					ItemSlot.SwapEquip(ref Main.mouseItem);
					if (Main.player[Main.myPlayer].ItemSpace(Main.mouseItem).CanTakeItemToPersonalInventory)
					{
						Main.mouseItem = player.GetItem(player.whoAmI, Main.mouseItem, GetItemSettings.InventoryUIToInventorySettings);
					}
					PlayerInput.LockGamepadButtons("Grapple");
					PlayerInput.SettingsForUI.TryRevertingToMouseMode();
				}
			}
			bool flag9 = Main.mouseItem.stack <= 0;
			if (flag9 || (Main.mouseItem.type == createItem.type && Main.mouseItem.stack < Main.mouseItem.maxStack))
			{
				text = ((!flag9) ? (text + PlayerInput.BuildCommand(Lang.misc[72].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"])) : (text + PlayerInput.BuildCommand(Lang.misc[72].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"], PlayerInput.ProfileGamepadUI.KeyStatus["MouseRight"])));
			}
			if (!flag9 && Main.mouseItem.type == createItem.type && Main.mouseItem.stack < Main.mouseItem.maxStack)
			{
				text += PlayerInput.BuildCommand(Lang.misc[93].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseRight"]);
			}
			if (flag8)
			{
				text += PlayerInput.BuildCommand(Lang.misc[71].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]);
			}
			return text + HandleItem2();
		};
		cp10.ReachEndEvent += delegate(int current, int next)
		{
			switch (current)
			{
			case 1501:
				switch (next)
				{
				case -1:
					if (Main.focusRecipe > 0)
					{
						Main.focusRecipe--;
					}
					break;
				case -2:
					if (Main.focusRecipe < Main.numAvailableRecipes - 1)
					{
						Main.focusRecipe++;
					}
					break;
				}
				break;
			default:
				switch (next)
				{
				case -1:
					if (Main.focusRecipe > 0)
					{
						UILinkPointNavigator.ChangePoint(1501);
						Main.focusRecipe--;
					}
					break;
				case -2:
					if (Main.focusRecipe < Main.numAvailableRecipes - 1)
					{
						UILinkPointNavigator.ChangePoint(1501);
						Main.focusRecipe++;
					}
					break;
				}
				break;
			case 1500:
				break;
			}
		};
		cp10.EnterEvent += delegate
		{
			Main.recBigList = false;
			PlayerInput.LockGamepadButtons("MouseLeft");
		};
		cp10.CanEnterEvent += () => Main.playerInventory && (Main.numAvailableRecipes > 0 || Main.InGuideCraftMenu);
		cp10.IsValidEvent += () => Main.playerInventory && (Main.numAvailableRecipes > 0 || Main.InGuideCraftMenu);
		cp10.PageOnLeft = 10;
		cp10.PageOnRight = 0;
		UILinkPointNavigator.RegisterPage(cp10, 9);
		UILinkPage cp11 = new UILinkPage();
		cp11.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		for (int num34 = 700; num34 < 1500; num34++)
		{
			UILinkPoint uILinkPoint12 = new UILinkPoint(num34, enabled: true, num34, num34, num34, num34);
			int IHateLambda = num34;
			uILinkPoint12.OnSpecialInteracts += delegate
			{
				string text2 = "";
				bool flag10 = false;
				Player player2 = Main.player[Main.myPlayer];
				if (IHateLambda + Main.recStart < Main.numAvailableRecipes)
				{
					int num35 = Main.recStart + IHateLambda - 700;
					if (Main.mouseItem.type == 0 && Main.recipe[Main.availableRecipe[num35]].createItem.maxStack > 1 && player2.ItemSpace(Main.recipe[Main.availableRecipe[num35]].createItem).CanTakeItemToPersonalInventory && !player2.HasLockedInventory())
					{
						flag10 = true;
						if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.Grapple)
						{
							SomeVarsForUILinkers.SequencedCraftingCurrent = Main.recipe[Main.availableRecipe[num35]];
						}
						if (CanExecuteInputCommand() && PlayerInput.Triggers.Current.Grapple && Main.stackSplit <= 1)
						{
							ItemSlot.RefreshStackSplitCooldown();
							if (SomeVarsForUILinkers.SequencedCraftingCurrent == Main.recipe[Main.availableRecipe[num35]])
							{
								Main.CraftItem(Main.recipe[Main.availableRecipe[num35]]);
								Main.mouseItem = player2.GetItem(player2.whoAmI, Main.mouseItem, GetItemSettings.InventoryUIToInventorySettings);
							}
						}
					}
				}
				text2 += PlayerInput.BuildCommand(Lang.misc[73].Value, !flag10, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
				if (flag10)
				{
					text2 += PlayerInput.BuildCommand(Lang.misc[71].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]);
				}
				return text2;
			};
			cp11.LinkMap.Add(num34, uILinkPoint12);
		}
		cp11.UpdateEvent += delegate
		{
			int num36 = UILinkPointNavigator.Shortcuts.CRAFT_IconsPerRow;
			int cRAFT_IconsPerColumn = UILinkPointNavigator.Shortcuts.CRAFT_IconsPerColumn;
			if (num36 == 0)
			{
				num36 = 100;
			}
			int num37 = num36 * cRAFT_IconsPerColumn;
			if (num37 > 800)
			{
				num37 = 800;
			}
			if (num37 > Main.numAvailableRecipes)
			{
				num37 = Main.numAvailableRecipes;
			}
			for (int num38 = 0; num38 < num37; num38++)
			{
				cp11.LinkMap[700 + num38].Left = ((num38 % num36 == 0) ? (-3) : (700 + num38 - 1));
				cp11.LinkMap[700 + num38].Right = (((num38 + 1) % num36 == 0 || num38 == Main.numAvailableRecipes - 1) ? (-4) : (700 + num38 + 1));
				cp11.LinkMap[700 + num38].Down = ((num38 < num37 - num36) ? (700 + num38 + num36) : (-2));
				cp11.LinkMap[700 + num38].Up = ((num38 < num36) ? (-1) : (700 + num38 - num36));
			}
			cp11.PageOnLeft = GetCornerWrapPageIdFromLeftToRight();
		};
		cp11.ReachEndEvent += delegate(int current, int next)
		{
			int cRAFT_IconsPerRow = UILinkPointNavigator.Shortcuts.CRAFT_IconsPerRow;
			switch (next)
			{
			case -1:
				Main.recStart -= cRAFT_IconsPerRow;
				if (Main.recStart < 0)
				{
					Main.recStart = 0;
				}
				break;
			case -2:
				Main.recStart += cRAFT_IconsPerRow;
				SoundEngine.PlaySound(12);
				if (Main.recStart > Main.numAvailableRecipes - cRAFT_IconsPerRow)
				{
					Main.recStart = Main.numAvailableRecipes - cRAFT_IconsPerRow;
				}
				break;
			}
		};
		cp11.EnterEvent += delegate
		{
			Main.recBigList = true;
		};
		cp11.LeaveEvent += delegate
		{
			Main.recBigList = false;
		};
		cp11.CanEnterEvent += () => Main.playerInventory && Main.numAvailableRecipes > 0;
		cp11.IsValidEvent += () => Main.playerInventory && Main.recBigList && Main.numAvailableRecipes > 0;
		cp11.PageOnLeft = 0;
		cp11.PageOnRight = 9;
		UILinkPointNavigator.RegisterPage(cp11, 10);
		UILinkPage cp12 = new UILinkPage();
		cp12.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		for (int num39 = 2605; num39 < 2620; num39++)
		{
			UILinkPoint uILinkPoint13 = new UILinkPoint(num39, enabled: true, num39, num39, num39, num39);
			uILinkPoint13.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[73].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
			cp12.LinkMap.Add(num39, uILinkPoint13);
		}
		cp12.UpdateEvent += delegate
		{
			int num40 = 5;
			int num41 = 3;
			int num42 = num40 * num41;
			int count = Main.Hairstyles.AvailableHairstyles.Count;
			for (int num43 = 0; num43 < num42; num43++)
			{
				cp12.LinkMap[2605 + num43].Left = ((num43 % num40 == 0) ? (-3) : (2605 + num43 - 1));
				cp12.LinkMap[2605 + num43].Right = (((num43 + 1) % num40 == 0 || num43 == count - 1) ? (-4) : (2605 + num43 + 1));
				cp12.LinkMap[2605 + num43].Down = ((num43 < num42 - num40) ? (2605 + num43 + num40) : (-2));
				cp12.LinkMap[2605 + num43].Up = ((num43 < num40) ? (-1) : (2605 + num43 - num40));
			}
		};
		cp12.ReachEndEvent += delegate(int current, int next)
		{
			int num44 = 5;
			switch (next)
			{
			case -1:
				Main.hairStart -= num44;
				SoundEngine.PlaySound(12);
				break;
			case -2:
				Main.hairStart += num44;
				SoundEngine.PlaySound(12);
				break;
			}
		};
		cp12.CanEnterEvent += () => Main.hairWindow;
		cp12.IsValidEvent += () => Main.hairWindow;
		cp12.PageOnLeft = 12;
		cp12.PageOnRight = 12;
		UILinkPointNavigator.RegisterPage(cp12, 11);
		UILinkPage uILinkPage6 = new UILinkPage();
		uILinkPage6.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		uILinkPage6.LinkMap.Add(2600, new UILinkPoint(2600, enabled: true, -3, -4, -1, 2601));
		uILinkPage6.LinkMap.Add(2601, new UILinkPoint(2601, enabled: true, -3, -4, 2600, 2602));
		uILinkPage6.LinkMap.Add(2602, new UILinkPoint(2602, enabled: true, -3, -4, 2601, 2603));
		uILinkPage6.LinkMap.Add(2603, new UILinkPoint(2603, enabled: true, -3, 2604, 2602, -2));
		uILinkPage6.LinkMap.Add(2604, new UILinkPoint(2604, enabled: true, 2603, -4, 2602, -2));
		uILinkPage6.UpdateEvent += delegate
		{
			Vector3 value21 = Main.rgbToHsl(Main.selColor);
			float interfaceDeadzoneX = PlayerInput.CurrentProfile.InterfaceDeadzoneX;
			float x = PlayerInput.GamepadThumbstickLeft.X;
			x = ((!(x < 0f - interfaceDeadzoneX) && !(x > interfaceDeadzoneX)) ? 0f : (MathHelper.Lerp(0f, 1f / 120f, (Math.Abs(x) - interfaceDeadzoneX) / (1f - interfaceDeadzoneX)) * (float)Math.Sign(x)));
			int currentPoint4 = UILinkPointNavigator.CurrentPoint;
			if (currentPoint4 == 2600)
			{
				Main.hBar = MathHelper.Clamp(Main.hBar + x, 0f, 1f);
			}
			if (currentPoint4 == 2601)
			{
				Main.sBar = MathHelper.Clamp(Main.sBar + x, 0f, 1f);
			}
			if (currentPoint4 == 2602)
			{
				Main.lBar = MathHelper.Clamp(Main.lBar + x, 0.15f, 1f);
			}
			Vector3.Clamp(value21, Vector3.Zero, Vector3.One);
			if (x != 0f)
			{
				if (Main.hairWindow)
				{
					Main.player[Main.myPlayer].hairColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
				}
				SoundEngine.PlaySound(12);
			}
		};
		uILinkPage6.CanEnterEvent += () => Main.hairWindow;
		uILinkPage6.IsValidEvent += () => Main.hairWindow;
		uILinkPage6.PageOnLeft = 11;
		uILinkPage6.PageOnRight = 11;
		UILinkPointNavigator.RegisterPage(uILinkPage6, 12);
		UILinkPage cp13 = new UILinkPage();
		for (int num45 = 0; num45 < 30; num45++)
		{
			cp13.LinkMap.Add(2900 + num45, new UILinkPoint(2900 + num45, enabled: true, -3, -4, -1, -2));
			cp13.LinkMap[2900 + num45].OnSpecialInteracts += value;
		}
		cp13.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		cp13.TravelEvent += delegate
		{
			if (UILinkPointNavigator.CurrentPage == cp13.ID)
			{
				int num46 = cp13.CurrentPoint - 2900;
				if (num46 < 5)
				{
					IngameOptions.category = num46;
				}
			}
		};
		cp13.UpdateEvent += delegate
		{
			int num47 = UILinkPointNavigator.Shortcuts.INGAMEOPTIONS_BUTTONS_LEFT;
			if (num47 == 0)
			{
				num47 = 5;
			}
			if (UILinkPointNavigator.OverridePoint == -1 && cp13.CurrentPoint < 2930 && cp13.CurrentPoint > 2900 + num47 - 1)
			{
				UILinkPointNavigator.ChangePoint(2900);
			}
			for (int num48 = 2900; num48 < 2900 + num47; num48++)
			{
				cp13.LinkMap[num48].Up = num48 - 1;
				cp13.LinkMap[num48].Down = num48 + 1;
			}
			cp13.LinkMap[2900].Up = 2900 + num47 - 1;
			cp13.LinkMap[2900 + num47 - 1].Down = 2900;
			int num49 = cp13.CurrentPoint - 2900;
			if (num49 < 4 && CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.MouseLeft)
			{
				IngameOptions.category = num49;
				UILinkPointNavigator.ChangePage(1002);
			}
			int num50 = ((SocialAPI.Network != null && SocialAPI.Network.CanInvite()) ? 1 : 0);
			if (num49 == 4 + num50 && CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.MouseLeft)
			{
				UILinkPointNavigator.ChangePage(1004);
			}
		};
		cp13.EnterEvent += delegate
		{
			cp13.CurrentPoint = 2900 + IngameOptions.category;
		};
		cp13.PageOnLeft = (cp13.PageOnRight = 1002);
		cp13.IsValidEvent += () => Main.ingameOptionsWindow && !Main.InGameUI.IsVisible;
		cp13.CanEnterEvent += () => Main.ingameOptionsWindow && !Main.InGameUI.IsVisible;
		UILinkPointNavigator.RegisterPage(cp13, 1001);
		UILinkPage cp14 = new UILinkPage();
		for (int num51 = 0; num51 < 30; num51++)
		{
			cp14.LinkMap.Add(2930 + num51, new UILinkPoint(2930 + num51, enabled: true, -3, -4, -1, -2));
			cp14.LinkMap[2930 + num51].OnSpecialInteracts += value;
		}
		cp14.EnterEvent += delegate
		{
			Main.mouseLeftRelease = false;
		};
		cp14.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		cp14.UpdateEvent += delegate
		{
			int num52 = UILinkPointNavigator.Shortcuts.INGAMEOPTIONS_BUTTONS_RIGHT;
			if (num52 == 0)
			{
				num52 = 5;
			}
			if (UILinkPointNavigator.OverridePoint == -1 && cp14.CurrentPoint >= 2930 && cp14.CurrentPoint > 2930 + num52 - 1)
			{
				UILinkPointNavigator.ChangePoint(2930);
			}
			for (int num53 = 2930; num53 < 2930 + num52; num53++)
			{
				cp14.LinkMap[num53].Up = num53 - 1;
				cp14.LinkMap[num53].Down = num53 + 1;
			}
			cp14.LinkMap[2930].Up = -1;
			cp14.LinkMap[2930 + num52 - 1].Down = -2;
			HandleOptionsSpecials();
		};
		cp14.PageOnLeft = (cp14.PageOnRight = 1001);
		cp14.IsValidEvent += () => Main.ingameOptionsWindow;
		cp14.CanEnterEvent += () => Main.ingameOptionsWindow;
		UILinkPointNavigator.RegisterPage(cp14, 1002);
		UILinkPage cp15 = new UILinkPage();
		cp15.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		for (int num54 = 1550; num54 < 1558; num54++)
		{
			UILinkPoint uILinkPoint14 = new UILinkPoint(num54, enabled: true, -3, -4, -1, -2);
			switch (num54)
			{
			case 1551:
			case 1553:
			case 1555:
				uILinkPoint14.Up = uILinkPoint14.ID - 2;
				uILinkPoint14.Down = uILinkPoint14.ID + 2;
				uILinkPoint14.Right = uILinkPoint14.ID + 1;
				break;
			case 1552:
			case 1554:
			case 1556:
				uILinkPoint14.Up = uILinkPoint14.ID - 2;
				uILinkPoint14.Down = uILinkPoint14.ID + 2;
				uILinkPoint14.Left = uILinkPoint14.ID - 1;
				break;
			}
			cp15.LinkMap.Add(num54, uILinkPoint14);
		}
		cp15.LinkMap[1550].Down = 1551;
		cp15.LinkMap[1550].Right = 120;
		cp15.LinkMap[1550].Up = 307;
		cp15.LinkMap[1551].Up = 1550;
		cp15.LinkMap[1552].Up = 1550;
		cp15.LinkMap[1552].Right = 121;
		cp15.LinkMap[1554].Right = 121;
		cp15.LinkMap[1555].Down = 1570;
		cp15.LinkMap[1556].Down = 1570;
		cp15.LinkMap[1556].Right = 122;
		cp15.LinkMap[1557].Up = 1570;
		cp15.LinkMap[1557].Down = 308;
		cp15.LinkMap[1557].Right = 127;
		cp15.LinkMap.Add(1570, new UILinkPoint(1570, enabled: true, -3, -4, -1, -2));
		cp15.LinkMap[1570].Up = 1555;
		cp15.LinkMap[1570].Down = 1557;
		cp15.LinkMap[1570].Right = 126;
		for (int num55 = 0; num55 < 7; num55++)
		{
			cp15.LinkMap[1550 + num55].OnSpecialInteracts += value;
		}
		cp15.UpdateEvent += delegate
		{
			if (!Main.ShouldPVPDraw)
			{
				if (UILinkPointNavigator.OverridePoint == -1 && cp15.CurrentPoint != 1557 && cp15.CurrentPoint != 1570)
				{
					UILinkPointNavigator.ChangePoint(1557);
				}
				cp15.LinkMap[1570].Up = -1;
				cp15.LinkMap[1557].Down = 308;
				cp15.LinkMap[1557].Right = 127;
			}
			else
			{
				cp15.LinkMap[1570].Up = 1555;
				cp15.LinkMap[1557].Down = 308;
				cp15.LinkMap[1557].Right = 127;
			}
			int iNFOACCCOUNT = UILinkPointNavigator.Shortcuts.INFOACCCOUNT;
			if (iNFOACCCOUNT > 0)
			{
				cp15.LinkMap[1570].Up = 1558 + (iNFOACCCOUNT - 1) / 2 * 2;
			}
			if (Main.ShouldPVPDraw)
			{
				if (iNFOACCCOUNT >= 1)
				{
					cp15.LinkMap[1555].Down = 1558;
					cp15.LinkMap[1556].Down = 1558;
				}
				else
				{
					cp15.LinkMap[1555].Down = 1570;
					cp15.LinkMap[1556].Down = 1570;
				}
				if (iNFOACCCOUNT >= 2)
				{
					cp15.LinkMap[1556].Down = 1559;
				}
				else
				{
					cp15.LinkMap[1556].Down = 1570;
				}
			}
		};
		cp15.IsValidEvent += () => Main.playerInventory;
		cp15.PageOnLeft = 8;
		cp15.PageOnRight = 8;
		UILinkPointNavigator.RegisterPage(cp15, 16);
		UILinkPage cp16 = new UILinkPage();
		cp16.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		for (int num56 = 1558; num56 < 1570; num56++)
		{
			UILinkPoint uILinkPoint15 = new UILinkPoint(num56, enabled: true, -3, -4, -1, -2);
			uILinkPoint15.OnSpecialInteracts += value;
			switch (num56)
			{
			case 1559:
			case 1561:
			case 1563:
				uILinkPoint15.Up = uILinkPoint15.ID - 2;
				uILinkPoint15.Down = uILinkPoint15.ID + 2;
				uILinkPoint15.Right = uILinkPoint15.ID + 1;
				break;
			case 1560:
			case 1562:
			case 1564:
				uILinkPoint15.Up = uILinkPoint15.ID - 2;
				uILinkPoint15.Down = uILinkPoint15.ID + 2;
				uILinkPoint15.Left = uILinkPoint15.ID - 1;
				break;
			}
			cp16.LinkMap.Add(num56, uILinkPoint15);
		}
		cp16.UpdateEvent += delegate
		{
			int iNFOACCCOUNT2 = UILinkPointNavigator.Shortcuts.INFOACCCOUNT;
			if (UILinkPointNavigator.OverridePoint == -1 && cp16.CurrentPoint - 1558 >= iNFOACCCOUNT2)
			{
				UILinkPointNavigator.ChangePoint(1558 + iNFOACCCOUNT2 - 1);
			}
			for (int num57 = 0; num57 < iNFOACCCOUNT2; num57++)
			{
				bool flag11 = num57 % 2 == 0;
				int num58 = num57 + 1558;
				cp16.LinkMap[num58].Down = ((num57 < iNFOACCCOUNT2 - 2) ? (num58 + 2) : 1570);
				cp16.LinkMap[num58].Up = ((num57 > 1) ? (num58 - 2) : (Main.ShouldPVPDraw ? (flag11 ? 1555 : 1556) : (-1)));
				cp16.LinkMap[num58].Right = ((flag11 && num57 + 1 < iNFOACCCOUNT2) ? (num58 + 1) : (123 + num57 / 4));
				cp16.LinkMap[num58].Left = (flag11 ? (-3) : (num58 - 1));
			}
		};
		cp16.IsValidEvent += () => Main.playerInventory && UILinkPointNavigator.Shortcuts.INFOACCCOUNT > 0;
		cp16.PageOnLeft = 8;
		cp16.PageOnRight = 8;
		UILinkPointNavigator.RegisterPage(cp16, 17);
		UILinkPage cp17 = new UILinkPage();
		cp17.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		for (int num59 = 6000; num59 < 6012; num59++)
		{
			UILinkPoint uILinkPoint16 = new UILinkPoint(num59, enabled: true, -3, -4, -1, -2);
			switch (num59)
			{
			case 6000:
				uILinkPoint16.Right = 0;
				break;
			case 6001:
			case 6002:
				uILinkPoint16.Right = 10;
				break;
			case 6003:
			case 6004:
				uILinkPoint16.Right = 20;
				break;
			case 6005:
			case 6006:
				uILinkPoint16.Right = 30;
				break;
			default:
				uILinkPoint16.Right = 40;
				break;
			}
			cp17.LinkMap.Add(num59, uILinkPoint16);
		}
		cp17.UpdateEvent += delegate
		{
			int bUILDERACCCOUNT = UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT;
			if (UILinkPointNavigator.OverridePoint == -1 && cp17.CurrentPoint - 6000 >= bUILDERACCCOUNT)
			{
				UILinkPointNavigator.ChangePoint(6000 + bUILDERACCCOUNT - 1);
			}
			for (int num60 = 0; num60 < bUILDERACCCOUNT; num60++)
			{
				_ = num60 % 2;
				int num61 = num60 + 6000;
				cp17.LinkMap[num61].Down = ((num60 < bUILDERACCCOUNT - 1) ? (num61 + 1) : (-2));
				cp17.LinkMap[num61].Up = ((num60 > 0) ? (num61 - 1) : (-1));
			}
		};
		cp17.IsValidEvent += () => Main.playerInventory && UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 0;
		cp17.PageOnLeft = 8;
		cp17.PageOnRight = 8;
		UILinkPointNavigator.RegisterPage(cp17, 18);
		UILinkPage uILinkPage7 = new UILinkPage();
		uILinkPage7.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		uILinkPage7.LinkMap.Add(2806, new UILinkPoint(2806, enabled: true, 2805, 2807, -1, 2808));
		uILinkPage7.LinkMap.Add(2807, new UILinkPoint(2807, enabled: true, 2806, 2810, -1, 2809));
		uILinkPage7.LinkMap.Add(2808, new UILinkPoint(2808, enabled: true, 2805, 2809, 2806, -2));
		uILinkPage7.LinkMap.Add(2809, new UILinkPoint(2809, enabled: true, 2808, 2811, 2807, -2));
		uILinkPage7.LinkMap.Add(2810, new UILinkPoint(2810, enabled: true, 2807, -4, -1, 2811));
		uILinkPage7.LinkMap.Add(2811, new UILinkPoint(2811, enabled: true, 2809, -4, 2810, -2));
		uILinkPage7.LinkMap.Add(2805, new UILinkPoint(2805, enabled: true, -3, 2806, -1, -2));
		uILinkPage7.LinkMap[2806].OnSpecialInteracts += value;
		uILinkPage7.LinkMap[2807].OnSpecialInteracts += value;
		uILinkPage7.LinkMap[2808].OnSpecialInteracts += value;
		uILinkPage7.LinkMap[2809].OnSpecialInteracts += value;
		uILinkPage7.LinkMap[2805].OnSpecialInteracts += value;
		uILinkPage7.CanEnterEvent += () => Main.clothesWindow;
		uILinkPage7.IsValidEvent += () => Main.clothesWindow;
		uILinkPage7.EnterEvent += delegate
		{
			Main.player[Main.myPlayer].releaseInventory = false;
		};
		uILinkPage7.LeaveEvent += delegate
		{
			Main.player[Main.myPlayer].LockGamepadTileInteractions();
		};
		uILinkPage7.PageOnLeft = 15;
		uILinkPage7.PageOnRight = 15;
		UILinkPointNavigator.RegisterPage(uILinkPage7, 14);
		UILinkPage uILinkPage8 = new UILinkPage();
		uILinkPage8.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		uILinkPage8.LinkMap.Add(2800, new UILinkPoint(2800, enabled: true, -3, -4, -1, 2801));
		uILinkPage8.LinkMap.Add(2801, new UILinkPoint(2801, enabled: true, -3, -4, 2800, 2802));
		uILinkPage8.LinkMap.Add(2802, new UILinkPoint(2802, enabled: true, -3, -4, 2801, 2803));
		uILinkPage8.LinkMap.Add(2803, new UILinkPoint(2803, enabled: true, -3, 2804, 2802, -2));
		uILinkPage8.LinkMap.Add(2804, new UILinkPoint(2804, enabled: true, 2803, -4, 2802, -2));
		uILinkPage8.LinkMap[2800].OnSpecialInteracts += value;
		uILinkPage8.LinkMap[2801].OnSpecialInteracts += value;
		uILinkPage8.LinkMap[2802].OnSpecialInteracts += value;
		uILinkPage8.LinkMap[2803].OnSpecialInteracts += value;
		uILinkPage8.LinkMap[2804].OnSpecialInteracts += value;
		uILinkPage8.UpdateEvent += delegate
		{
			Vector3 value22 = Main.rgbToHsl(Main.selColor);
			float interfaceDeadzoneX2 = PlayerInput.CurrentProfile.InterfaceDeadzoneX;
			float x2 = PlayerInput.GamepadThumbstickLeft.X;
			x2 = ((!(x2 < 0f - interfaceDeadzoneX2) && !(x2 > interfaceDeadzoneX2)) ? 0f : (MathHelper.Lerp(0f, 1f / 120f, (Math.Abs(x2) - interfaceDeadzoneX2) / (1f - interfaceDeadzoneX2)) * (float)Math.Sign(x2)));
			int currentPoint5 = UILinkPointNavigator.CurrentPoint;
			if (currentPoint5 == 2800)
			{
				Main.hBar = MathHelper.Clamp(Main.hBar + x2, 0f, 1f);
			}
			if (currentPoint5 == 2801)
			{
				Main.sBar = MathHelper.Clamp(Main.sBar + x2, 0f, 1f);
			}
			if (currentPoint5 == 2802)
			{
				Main.lBar = MathHelper.Clamp(Main.lBar + x2, 0.15f, 1f);
			}
			Vector3.Clamp(value22, Vector3.Zero, Vector3.One);
			if (x2 != 0f)
			{
				if (Main.clothesWindow)
				{
					Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
					switch (Main.selClothes)
					{
					case 0:
						Main.player[Main.myPlayer].shirtColor = Main.selColor;
						break;
					case 1:
						Main.player[Main.myPlayer].underShirtColor = Main.selColor;
						break;
					case 2:
						Main.player[Main.myPlayer].pantsColor = Main.selColor;
						break;
					case 3:
						Main.player[Main.myPlayer].shoeColor = Main.selColor;
						break;
					}
				}
				SoundEngine.PlaySound(12);
			}
		};
		uILinkPage8.CanEnterEvent += () => Main.clothesWindow;
		uILinkPage8.IsValidEvent += () => Main.clothesWindow;
		uILinkPage8.EnterEvent += delegate
		{
			Main.player[Main.myPlayer].releaseInventory = false;
		};
		uILinkPage8.LeaveEvent += delegate
		{
			Main.player[Main.myPlayer].LockGamepadTileInteractions();
		};
		uILinkPage8.PageOnLeft = 14;
		uILinkPage8.PageOnRight = 14;
		UILinkPointNavigator.RegisterPage(uILinkPage8, 15);
		UILinkPage cp18 = new UILinkPage();
		cp18.UpdateEvent += delegate
		{
			PlayerInput.GamepadAllowScrolling = true;
		};
		for (int num62 = 3000; num62 <= 4999; num62++)
		{
			cp18.LinkMap.Add(num62, new UILinkPoint(num62, enabled: true, -3, -4, -1, -2));
		}
		cp18.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[53].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]) + PlayerInput.BuildCommand(Lang.misc[82].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + FancyUISpecialInstructions();
		cp18.UpdateEvent += delegate
		{
			if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.Inventory)
			{
				FancyExit();
			}
			UILinkPointNavigator.Shortcuts.BackButtonInUse = false;
		};
		cp18.EnterEvent += delegate
		{
			cp18.CurrentPoint = 3002;
		};
		cp18.CanEnterEvent += () => Main.MenuUI.IsVisible || Main.InGameUI.IsVisible;
		cp18.IsValidEvent += () => Main.MenuUI.IsVisible || Main.InGameUI.IsVisible;
		cp18.OnPageMoveAttempt += OnFancyUIPageMoveAttempt;
		UILinkPointNavigator.RegisterPage(cp18, 1004);
		UILinkPage cp19 = new UILinkPage();
		cp19.UpdateEvent += delegate
		{
			PlayerInput.GamepadAllowScrolling = true;
		};
		for (int num63 = 10000; num63 <= 11000; num63++)
		{
			cp19.LinkMap.Add(num63, new UILinkPoint(num63, enabled: true, -3, -4, -1, -2));
		}
		for (int num64 = 15000; num64 <= 15000; num64++)
		{
			cp19.LinkMap.Add(num64, new UILinkPoint(num64, enabled: true, -3, -4, -1, -2));
		}
		cp19.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[53].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]) + PlayerInput.BuildCommand(Lang.misc[82].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + FancyUISpecialInstructions();
		cp19.UpdateEvent += delegate
		{
			if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.Inventory)
			{
				FancyExit();
			}
			UILinkPointNavigator.Shortcuts.BackButtonInUse = false;
		};
		cp19.EnterEvent += delegate
		{
			cp19.CurrentPoint = 10000;
		};
		cp19.CanEnterEvent += CanEnterCreativeMenu;
		cp19.IsValidEvent += CanEnterCreativeMenu;
		cp19.OnPageMoveAttempt += OnFancyUIPageMoveAttempt;
		cp19.PageOnLeft = 8;
		cp19.PageOnRight = 0;
		UILinkPointNavigator.RegisterPage(cp19, 1005);
		UILinkPage cp20 = new UILinkPage();
		cp20.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value23 = () => PlayerInput.BuildCommand(Lang.misc[94].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
		for (int num65 = 9000; num65 <= 9050; num65++)
		{
			UILinkPoint uILinkPoint17 = new UILinkPoint(num65, enabled: true, num65 + 10, num65 - 10, num65 - 1, num65 + 1);
			cp20.LinkMap.Add(num65, uILinkPoint17);
			uILinkPoint17.OnSpecialInteracts += value23;
		}
		cp20.UpdateEvent += delegate
		{
			int num66 = UILinkPointNavigator.Shortcuts.BUFFS_PER_COLUMN;
			if (num66 == 0)
			{
				num66 = 100;
			}
			for (int num67 = 0; num67 < 50; num67++)
			{
				cp20.LinkMap[9000 + num67].Up = ((num67 % num66 == 0) ? (-1) : (9000 + num67 - 1));
				if (cp20.LinkMap[9000 + num67].Up == -1)
				{
					if (num67 >= num66)
					{
						cp20.LinkMap[9000 + num67].Up = 184;
					}
					else
					{
						cp20.LinkMap[9000 + num67].Up = 189;
					}
				}
				cp20.LinkMap[9000 + num67].Down = (((num67 + 1) % num66 == 0 || num67 == UILinkPointNavigator.Shortcuts.BUFFS_DRAWN - 1) ? 308 : (9000 + num67 + 1));
				cp20.LinkMap[9000 + num67].Left = ((num67 < UILinkPointNavigator.Shortcuts.BUFFS_DRAWN - num66) ? (9000 + num67 + num66) : (-3));
				cp20.LinkMap[9000 + num67].Right = ((num67 < num66) ? (-4) : (9000 + num67 - num66));
			}
		};
		cp20.IsValidEvent += () => Main.playerInventory && Main.EquipPage == 2 && UILinkPointNavigator.Shortcuts.BUFFS_DRAWN > 0;
		cp20.PageOnLeft = 8;
		cp20.PageOnRight = 8;
		UILinkPointNavigator.RegisterPage(cp20, 19);
		UILinkPage uILinkPage9 = UILinkPointNavigator.Pages[UILinkPointNavigator.CurrentPage];
		uILinkPage9.CurrentPoint = uILinkPage9.DefaultPoint;
		uILinkPage9.Enter();
	}

	private static bool CanEnterCreativeMenu()
	{
		if (Main.LocalPlayer.chest != -1)
		{
			return false;
		}
		if (Main.LocalPlayer.talkNPC != -1)
		{
			return false;
		}
		if (Main.playerInventory)
		{
			return Main.CreativeMenu.Enabled;
		}
		return false;
	}

	private static int GetCornerWrapPageIdFromLeftToRight()
	{
		return 8;
	}

	private static int GetCornerWrapPageIdFromRightToLeft()
	{
		if (Main.CreativeMenu.Enabled)
		{
			return 1005;
		}
		return 10;
	}

	private static void OnFancyUIPageMoveAttempt(int direction)
	{
		if (Main.MenuUI.CurrentState is UICharacterCreation uICharacterCreation)
		{
			uICharacterCreation.TryMovingCategory(direction);
		}
		if (UserInterface.ActiveInstance.CurrentState is UIBestiaryTest uIBestiaryTest)
		{
			uIBestiaryTest.TryMovingPages(direction);
		}
	}

	public static void FancyExit()
	{
		switch (UILinkPointNavigator.Shortcuts.BackButtonCommand)
		{
		case 1:
			SoundEngine.PlaySound(11);
			Main.menuMode = 0;
			break;
		case 2:
			SoundEngine.PlaySound(11);
			Main.menuMode = ((!Main.menuMultiplayer) ? 1 : 12);
			break;
		case 3:
			Main.menuMode = 0;
			IngameFancyUI.Close();
			break;
		case 4:
			SoundEngine.PlaySound(11);
			Main.menuMode = 11;
			break;
		case 5:
			SoundEngine.PlaySound(11);
			Main.menuMode = 11;
			break;
		case 6:
			UIVirtualKeyboard.Cancel();
			break;
		case 7:
			if (Main.MenuUI.CurrentState is IHaveBackButtonCommand haveBackButtonCommand)
			{
				haveBackButtonCommand.HandleBackButtonUsage();
			}
			break;
		}
	}

	public static string FancyUISpecialInstructions()
	{
		string text = "";
		int fANCYUI_SPECIAL_INSTRUCTIONS = UILinkPointNavigator.Shortcuts.FANCYUI_SPECIAL_INSTRUCTIONS;
		if (fANCYUI_SPECIAL_INSTRUCTIONS == 1)
		{
			if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.HotbarMinus)
			{
				UIVirtualKeyboard.CycleSymbols();
				PlayerInput.LockGamepadButtons("HotbarMinus");
				PlayerInput.SettingsForUI.TryRevertingToMouseMode();
			}
			text += PlayerInput.BuildCommand(Lang.menu[235].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"]);
			if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.MouseRight)
			{
				UIVirtualKeyboard.BackSpace();
				PlayerInput.LockGamepadButtons("MouseRight");
				PlayerInput.SettingsForUI.TryRevertingToMouseMode();
			}
			text += PlayerInput.BuildCommand(Lang.menu[236].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseRight"]);
			if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.SmartCursor)
			{
				UIVirtualKeyboard.Write(" ");
				PlayerInput.LockGamepadButtons("SmartCursor");
				PlayerInput.SettingsForUI.TryRevertingToMouseMode();
			}
			text += PlayerInput.BuildCommand(Lang.menu[238].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["SmartCursor"]);
			if (UIVirtualKeyboard.CanSubmit)
			{
				if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.HotbarPlus)
				{
					UIVirtualKeyboard.Submit();
					PlayerInput.LockGamepadButtons("HotbarPlus");
					PlayerInput.SettingsForUI.TryRevertingToMouseMode();
				}
				text += PlayerInput.BuildCommand(Lang.menu[237].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
			}
		}
		return text;
	}

	public static void HandleOptionsSpecials()
	{
		switch (UILinkPointNavigator.Shortcuts.OPTIONS_BUTTON_SPECIALFEATURE)
		{
		case 1:
			Main.bgScroll = (int)HandleSliderHorizontalInput(Main.bgScroll, 0f, 100f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 1f);
			Main.caveParallax = 1f - (float)Main.bgScroll / 500f;
			break;
		case 2:
			Main.musicVolume = HandleSliderHorizontalInput(Main.musicVolume, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
			break;
		case 3:
			Main.soundVolume = HandleSliderHorizontalInput(Main.soundVolume, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
			break;
		case 4:
			Main.ambientVolume = HandleSliderHorizontalInput(Main.ambientVolume, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
			break;
		case 5:
		{
			float hBar = Main.hBar;
			float num3 = (Main.hBar = HandleSliderHorizontalInput(hBar, 0f, 1f));
			if (hBar != num3)
			{
				switch (Main.menuMode)
				{
				case 17:
					Main.player[Main.myPlayer].hairColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 18:
					Main.player[Main.myPlayer].eyeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 19:
					Main.player[Main.myPlayer].skinColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 21:
					Main.player[Main.myPlayer].shirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 22:
					Main.player[Main.myPlayer].underShirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 23:
					Main.player[Main.myPlayer].pantsColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 24:
					Main.player[Main.myPlayer].shoeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 25:
					Main.mouseColorSlider.Hue = num3;
					break;
				case 252:
					Main.mouseBorderColorSlider.Hue = num3;
					break;
				}
				SoundEngine.PlaySound(12);
			}
			break;
		}
		case 6:
		{
			float sBar = Main.sBar;
			float num2 = (Main.sBar = HandleSliderHorizontalInput(sBar, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX));
			if (sBar != num2)
			{
				switch (Main.menuMode)
				{
				case 17:
					Main.player[Main.myPlayer].hairColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 18:
					Main.player[Main.myPlayer].eyeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 19:
					Main.player[Main.myPlayer].skinColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 21:
					Main.player[Main.myPlayer].shirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 22:
					Main.player[Main.myPlayer].underShirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 23:
					Main.player[Main.myPlayer].pantsColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 24:
					Main.player[Main.myPlayer].shoeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 25:
					Main.mouseColorSlider.Saturation = num2;
					break;
				case 252:
					Main.mouseBorderColorSlider.Saturation = num2;
					break;
				}
				SoundEngine.PlaySound(12);
			}
			break;
		}
		case 7:
		{
			float lBar = Main.lBar;
			float min = 0.15f;
			if (Main.menuMode == 252)
			{
				min = 0f;
			}
			Main.lBar = HandleSliderHorizontalInput(lBar, min, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX);
			float lBar2 = Main.lBar;
			if (lBar != lBar2)
			{
				switch (Main.menuMode)
				{
				case 17:
					Main.player[Main.myPlayer].hairColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 18:
					Main.player[Main.myPlayer].eyeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 19:
					Main.player[Main.myPlayer].skinColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 21:
					Main.player[Main.myPlayer].shirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 22:
					Main.player[Main.myPlayer].underShirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 23:
					Main.player[Main.myPlayer].pantsColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 24:
					Main.player[Main.myPlayer].shoeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 25:
					Main.mouseColorSlider.Luminance = lBar2;
					break;
				case 252:
					Main.mouseBorderColorSlider.Luminance = lBar2;
					break;
				}
				SoundEngine.PlaySound(12);
			}
			break;
		}
		case 8:
		{
			float aBar = Main.aBar;
			float num4 = (Main.aBar = HandleSliderHorizontalInput(aBar, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX));
			if (aBar != num4)
			{
				int menuMode = Main.menuMode;
				if (menuMode == 252)
				{
					Main.mouseBorderColorSlider.Alpha = num4;
				}
				SoundEngine.PlaySound(12);
			}
			break;
		}
		case 9:
		{
			bool left = PlayerInput.Triggers.Current.Left;
			bool right = PlayerInput.Triggers.Current.Right;
			if (PlayerInput.Triggers.JustPressed.Left || PlayerInput.Triggers.JustPressed.Right)
			{
				SomeVarsForUILinkers.HairMoveCD = 0;
			}
			else if (SomeVarsForUILinkers.HairMoveCD > 0)
			{
				SomeVarsForUILinkers.HairMoveCD--;
			}
			if (SomeVarsForUILinkers.HairMoveCD == 0 && (left || right))
			{
				if (left)
				{
					Main.PendingPlayer.hair--;
				}
				if (right)
				{
					Main.PendingPlayer.hair++;
				}
				SomeVarsForUILinkers.HairMoveCD = 12;
			}
			int num = 51;
			if (Main.PendingPlayer.hair >= num)
			{
				Main.PendingPlayer.hair = 0;
			}
			if (Main.PendingPlayer.hair < 0)
			{
				Main.PendingPlayer.hair = num - 1;
			}
			break;
		}
		case 10:
			Main.GameZoomTarget = HandleSliderHorizontalInput(Main.GameZoomTarget, 1f, 2f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
			break;
		case 11:
			Main.UIScale = HandleSliderHorizontalInput(Main.UIScaleWanted, 0.5f, 2f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
			Main.temporaryGUIScaleSlider = Main.UIScaleWanted;
			break;
		case 12:
			Main.MapScale = HandleSliderHorizontalInput(Main.MapScale, 0.5f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.7f);
			break;
		}
	}
}
