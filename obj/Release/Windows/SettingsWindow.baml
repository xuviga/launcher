   M S B A M L   `   `   `  ����   iMineLauncher+    %iMine.Launcher.Windows.SettingsWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006       � Title$  iMine Настройки�� � 
ResizeMode$ NoResize=� � ShowInTaskbar .  $	��500��$	��383�� � WindowStyle$
 None=� � AllowsTransparency . $y�Visible=�
�������.��(� $
��Left=�.��� $	��450��$	��346��.+�� $��22��$��30,154,60,274q�.+��� $��0,8,0,8q�����  �� 
StartPoint$	 0,0&� �� EndPoint$	 1,0&�.���  � Color$ Black�� � Offset$ 0Q�� -   RamPointMin��$ DarkRed��$	 0.5Q�� -   RamPointMid��$ Orange��$	 0.6Q�� -   RamPointRec��$ Green��$	 0.7Q�m� -   $��MemoryTickBar��	 m� Maximum$		 100Q�
 m� Fill
 �#31221d m� 	Placement$ Bottom=�$��0,10,0,3q� m� TickFrequency$ 0Q�m� -   $��MemoryTickBar2��$		 100Q�
 �#31221d$	 Top=�$��0,3,0,10q�$ 0Q��� -   $��MemorySlider��   Horizontal_Slider#���    � Value$ 15Q��� -    ��Name$ MemoryValue��$��??��$��Right=�$	��Top=�$��Right=�$��Center=�$��	0,89,20,0q����#31221d � ToolTip �� ��Стоит учесть, что по факту Java потребляет больше памяти в силу своего строения�� \ZРазмер переиспользования может достигать 500-1000МБ�� -    ��Name$ MemoryWarning�� �� SelectionOpacity$ 0Q� �� 
IsReadOnly . $��0q�
������ $��Center=�$��25,211,25,0q�$	��Top=�$��11(�$	��122��$��Center=�. �/� .T�J� �� -	    ��Name$ ApplyButton�� �� IsCancel .  �� 	IsDefault . $��Закрыть��  BlackButtons#���  $	��100��$��50��$��Center=�$��Bottom=�$��123,0,123,67q��� -
   $��CheckerOpenConsoleOnPlay�� d� 	IsChecked$ FalseY�$H��BОтображать консоль при запуске игры��$��30��$��14(����#31221d$
��Left=�$��22,181,0,239q��� $)��#Выделение памяти [?]��$
��Left=����#31221d$	��Top=�$
��Left=�$��Center=�$��	20,89,0,0q� �� ��Стоит учесть, что по факту Java потребляет больше памяти в силу своего строения�� \ZРазмер переиспользования может достигать 500-1000МБ�� $��НАСТРОЙКИ��$��Center=����#d0b499$	��Top=�$��0,23,0,0q�$
��Bold"�$��18(��� -    ��Name$ CloseButton��$��Right=�$	��Top=�$��28��$��28��$��0,8,10,0q��� $
��Left=�$��
30,115,0,0q�$
��Wrap=�$����Задание памяти вручную отключает автоопределение. Используйте только если знаете, что именно вы делаете��$	��Top=�$	��296��$��10(��� -   $��
Сброс��$
��Left=�$��295,156,0,0q�$	��Top=�$��31��$��9(��� -   $��Логи��$
��Left=�$��
25,293,0,0q�$	��Top=�$��56��$��9(��� -   $��Скриншоты��$
��Left=�$��
25,315,0,0q�$	��Top=�$��56��$��9(�