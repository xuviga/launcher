   M S B A M L   `   `   `  ����   iMineLauncher'    !iMine.Launcher.Windows.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -     � Title$  iMine��$	��610��$
��1122�� � WindowStartupLocation$ CenterScreen=� � WindowStyle$
 None=� � AllowsTransparency . ���	#00000000 � 
ResizeMode$ NoResize=�.��(� $
��Left=�.��� -    �Name$ MainGrid��$
��Left=�$	��610��$	��Top=�$
��1122��.+��� -   
������ $��70��$	��Top=�$��	10,0,10,0q��� -   $��MainSliderBorder��$
��Left=�$	��305��$��215,140,0,0q�$	��Top=�$	��610�� �� Padding$ 0q����	#C03A2F2C���#423632$��3,3,3,3q�._�� -   $ 
MainSlider��.+��� -    ��Name$ PrevSlideButton��$��<��$
��Left=�$��5,0,0,0q�$��Center=�$��30��$��60��$��20(�   BigSlideButton#��[  $
��Bold"�$��Italic$��� -   $ NextSlideButton��$��>��$��Right=�$��0,0,5,0q�$��Center=�$��30��$��60��$��20(�#��[  $��Italic$�$
��Bold"��� -   $��MainSliderText�� �� 
IsReadOnly . 	 �� SelectionOpacity$	 0Q�$
��Auto��$��Bottom=�$	��610�����	#C03A2F2C���	#FFC4B7A4$��0q�$��5q��� -	   
 ��Name$
 SliderOpacity�� J� IsHitTestVisible .  ����  �� Color$ Black���� $��215,460,0,0q�$ 0q����	#C03A2F2C���#423632$��3,3,3,3q�$	��610��$	��110��$
��Left=�$	��Top=�._�� $��Right=�$��-3,-3,-3,-3q�$	��610��.+�� -
   $ BottomSlider0��$��90��$	��180��$
��Left=�$��
30,10,0,10q�.+��� -    ��Name$ BottomSliderText0�� .  $	 0Q� . $
��Auto��$��Bottom=����	#C03A2F2C���	#FFC4B7A4$��0q�$��1q�$��
-1,0,-1,-1q��� -    ��Name$ BottomSliderNew0��$��35��$��20��$��Right=�$	��Top=� .  ���	#FFFFB34F$
��NEW!��$
��Bold"�$y�Hidden=��� -   $
 BottomSliderOpacity1�� .  ���� $ Black���� -   $
 BottomSlider0Tint�� .   J� Opacity$ 0Q����� $ White��� -   $ BottomSlider1��$��90��$	��180��.+��� -   $ BottomSliderText1��$	 0Q� . $
��Auto��$��Bottom=����	#C03A2F2C���	#FFC4B7A4$��0q�$��1q�$��
-1,0,-1,-1q��� -   $ BottomSliderNew1��$��35��$��20��$��Right=�$	��Top=� .  ���	#FFFFB34F$
��NEW!��$
��Bold"�$y�Hidden=��� -   $
 BottomSliderOpacity2�� .  ���� $ Black���� -   $
 BottomSlider1Tint�� .  $ 0Q����� $ White��� -   $ BottomSlider2��$��90��$	��180��$��Right=�$��
0,10,30,10q�.+��� -   $ BottomSliderText2��$	 0Q� . $
��Auto��$��Bottom=����	#C03A2F2C���	#FFC4B7A4$��0q�$��1q�$��
-1,0,-1,-1q��� -   $ BottomSliderNew2��$��35��$��20��$��Right=�$	��Top=� .  ���	#FFFFB34F$
��NEW!��$
��Bold"�$y�Hidden=��� -   $
 BottomSliderOpacity3�� .  ���� $ Black���� -   $
 BottomSlider2Tint�� .  $ 0Q����� $ White���� -   $ PrevBottomButton��$��<��$
��Left=�$��5,0,0,0q�$��Center=�$��20��$��40��  SlideButton#��[ $
��Bold"�$��Italic$�$��16(�$	 0.5Q��� -   $ NextBottomButton��$��>��$��Right=�$��0,0,5,0q�$��Center=�$��20��$��40��#��[ $
��Bold"�$��Italic$�$��16(�$	 0.5Q��� $��
215,95,0,0q�$ 0q����	#C03A2F2C���#423632$��3,3,3,3q�$	��610��$��40��$
��Left=�$	��Top=�._�]� .+��� ���	#AAC4B7A4$��0q�$��25��$
��Left=� �� CornerRadius$ 3��$��	10,0,10,0q�$ 8,0,8,0q�._��� -    ��Name$ 
ServerName��$��[Error]��$��18(�$��Center=�$��Center=�$��-7,-6,-7,-4q��� -    ��Name$ 
TagBorder0�����	#55C4B7A4$��0q�$��18��$
��Left=�$ 5��$��0,0,5,0q�$ 6,0,5,0q�._��� -   $
 Tag0��$��[error]��$��14(�$��Center=�$��Center=�$��-7,-7,-7,-4q��� -   $ 
TagBorder1�����	#55C4B7A4$��0q�$��18��$
��Left=�$ 5��$��0,0,5,0q�$ 6,0,5,0q�._��� -   $
 Tag1��$��[error]��$��14(�$��Center=�$��Center=�$��-7,-7,-7,-4q��� -    $ 
TagBorder2�����	#55C4B7A4$��0q�$��18��$
��Left=�$ 5��$��0,0,5,0q�$ 6,0,5,0q�._��� -!   $
 Tag2��$��[error]��$��14(�$��Center=�$��Center=�$��-7,-7,-7,-4q��� -"   $ 
TagBorder3�����	#55C4B7A4$��0q�$��18��$
��Left=�$ 5��$��0,0,5,0q�$ 6,0,5,0q�._��� -#   $
 Tag3��$��[error]��$��14(�$��Center=�$��Center=�$��-7,-7,-7,-4q��� -$   $ 
TagBorder4�����	#55C4B7A4$��0q�$��18��$
��Left=�$ 5��$��0,0,5,0q�$ 6,0,5,0q�._��� -%   $
 Tag4��$��[error]��$��14(�$��Center=�$��Center=�$��-7,-7,-7,-4q��� -&   $ 
TagBorder5�����	#55C4B7A4$��0q�$��18��$
��Left=�$ 5��$��0,0,5,0q�$ 6,0,5,0q�._��� -'   $
 Tag5��$��[error]��$��14(�$��Center=�$��Center=�$��-7,-7,-7,-4q��� -(   $ 
TagBorder6�����	#55C4B7A4$��0q�$��18��$
��Left=�$ 5��$��0,0,5,0q�$ 6,0,5,0q�._��� -)   $
 Tag6��$��[error]��$��14(�$��Center=�$��Center=�$��-7,-7,-7,-4q��� -*   $ ServerInfoButton��$��О сервере��$��Right=�$	��Top=�$	��100��$��50��  BlackButtons#���  $��
0,94,296,0q��� -+    ��Name$
 Logo��$
��Left=�$��52��$��	32,15,0,0q�$	��Top=�$	��180���� -,   $ 
LoginLabel��$��[Error]��$y�Hidden=�$
��Left=�$��23��$��
808,28,0,0q�$
��Wrap=�$	��Top=�$	��120�����	#FF3A2F2C���	#FFC4B7A4 �� Padding$ 5,2,0,0q��� --   $��ConsoleButton��$	 0.5Q�$��32��$��32��  StartMenuButtons#���  $��217,23,873,555q��� -.   $��	BugButton��$	 0.5Q�$��32��$��32��#���  $��262,23,828,555q��� -/   $ LoginButton��$��
Логин��$
��Left=�$��
930,17,0,0q�$	��Top=�$	��100��$��50��#���   J� RenderTransformOrigin$ 0.498,0.162&��� -0   $ RegisterButton��$��Регистрация��$
��Left=�$��
837,17,0,0q�$	��Top=�$	��100��$��50��#���  �� -1   $ CloseButton��$��Right=�$	��Top=�$��54��$��28��$��	0,12,12,0q��� -2   $ ModListPanel��$
��Left=�$	��520��$��
825,82,0,0q�$	��Top=�$	��286�����	#C03A2F2C.+��� -3   $ ModList��$	 0Q� . 
������ $��14(�$��
15,20,15,0q����	#FFC4B7A4$��0q�" �� VerticalScrollBarVisibility$
 Auto=�$	��345�� � 	Resources  �� 
TargetType 
 Pingas �� BasedOn��%   '��       7  �� " ��8   �� -4   $��CheckListBorder��$��
20,5,20,15q�$��0��$��3q����	#77C84113._�� -5   $��CheckListGrid��$��-1,-1q����	#77A37537.+��� -6   $��CheckListTitle��$
��Left=�$	��Top=�$	 5,3q�$/��)Проблем не обнаружено!���� -7   $��CheckListButton��$��Right=�$	��Top=�$��-3q�$��15��$��15�����	#FF0F930F#���  $��Center=�$��0,4,5,0q�$��Center=�.���� -8   $��CheckListButtonText��$��X��$��0,0,0,0q��� -9   $��	CheckList��$��0,20,0,0q�$	 0Q� . 
������ $��14(����	#FFC4B7A4$��0q�$
 Auto=� .  ��%   '��       7  �� " ��8   ]� -:    ]�Name$ PlayButtonBcg��$��0,-28q�$��Bottom=�$��Center=�$	��211��$	��115��.+��� -;   $	��165��$��64��
������ #���  � .+��� -<   $ PlayerCount��$��Игроков: ?/?��$��16(�$��Center=����	#FFC4B7A4$��Center=�$��0,0,0,0q�$��Center=��� -=   $��SettingsButton��$	 0.5Q�$��32��$��32��#���  $��Right=�$��Bottom=�$��0,0,20,0q��� ->   $ ModPanel��$��540,170,0,0q�$y�Hidden=�$ 0q����	#F04A3F3C���#423632$��3q�$	��400��$��60��$
��Left=�$	��Top=�._�� 
������ $��-2q�.+��� -?   $��ModPanelBox��$��13(����	#FFC4B7A4$	 0Q� . 
������ $��0q�$	��Top=��� $��Right=�$��Bottom=�$	��6,4q��� -@    ��Name$ ModPanelLink��
����d�.�Подробнее�� -A   $ 	BlackTint�� K� ZIndex$ 99��$y�Hidden=�$	 0.7Q�