   M S B A M L   `   `   `  ����   iMineLauncher(    "iMine.Launcher.Windows.LoginWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006       � Title$  iMine Логин�� � ShowInTaskbar .  $	��400��$	��306�� � WindowStyle$
 None=� � AllowsTransparency . $y�Visible=����	#EE191511 � 
ResizeMode$ NoResize=�.��(� $
��Left=�.��� $��Center=�$	��Top=�$	��400��$	��306��.+��� $��
ЛОГИН��$��Center=����#d0b499$	��Top=�$��0,22,0,0q�$
��Bold"�$��18(��� -    ��Name$ CloseButton��   StartMenuButtons#���   $��Right=�$	��Top=�$��28��$��28��$��	0,16,12,0q� �� IsCancel . �� -    ��Name$ ErrorBox��$y�Hidden=����	#33FF0000
���  ��$
��Left=�$��	20,76,0,0q�$	��Top=�$	��274���� -   $ ResetPassButton��$��3��$
��Left=�$	��Top=�$	��126��$��63��  BlackButtons#���  $��
25,241,0,0q����� $ ��Забыли пароль?�����	#FFC4B7A4$��12(�$	��Top=�$��0,3,0,0q�$
��Left=�$��Center=�$��Center=��� -   $ NoAccountButton��$��4��$��Right=�$	��Top=�$	��126��$��63��#���  $��
0,241,25,0q����� $��Регистрация�����	#FFC4B7A4$��12(�$	��Top=�$��0,3,0,0q�$
��Left=�$��Center=�$��Center=��� $
��Left=�$��	35,92,0,0q�$
��Wrap=�$��
Логин��$��14(����	#FF563f35$	��Top=� J� RenderTransformOrigin$ 
0.74,0.609&��� $
��Left=�$��
35,166,0,0q�$
��Wrap=�$��Пароль��$��14(����	#FF563f35$	��Top=��� $��35,116,35,250q�$��2q�	 J� SnapsToDevicePixels	 . ���	#FF8e806e._��� $��2q�$��-1q����	#FFb6a18b._��� -   
 ��Name$
 LoginBox��$��0��$��-1q� �� 	MaxLength$ 16��$��0q����	#FFb6a18b$��Center=�$��Center=�$��15(����	#FF563f35.������  �� EndPoint$ 0.5,1&� �� 
StartPoint$ 0.5,0&�.���  � Color$ 	#FF8B7764�� � Offset$ 0Q�� $ 	#FFE5D8C5��$ 1Q�.���� $��35,190,35,176q�$��2q�	 . ���	#FF8e806e._��� $��2q�$��-1q����	#FFb6a18b._�G� -    G�Name$ PasswordBox��$��1��$��-1q�$��0q����	#FFb6a18b$��Center=�$��Center=�$��15(����	#FF563f35���� $ 0.5,1&�$ 0.5,0&�.��� $ 	#FF8B7764��$ 0Q�� $ 	#FFE5D8C5��$ 1Q��� -   $ LoginButton�� �� 	IsDefault . $��2��$	��172��$��86��$��67,304,67,10q�#���   ���� $��
Войти��
�������$��18(�$��	0,-18,0,0q�