   M S B A M L   `   `   `  ����   iMineLauncher    iMine.Launcher.App   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       �� 	Resources     StartMenuButtons �� 
TargetType �� Property �� Value �� 
TargetType  BlackButtons  BigSlideButton �� Padding  SlideButton  ScrollBarTrackThumb �Name ��Name �� CornerRadius	 �� Triggers
 P� Property P� Value �� 
TargetName 
 Pingas � RowDefinitions ]�Name ]� Thumb n�Name ]� IncreaseRepeatButton ��Name J� Opacity ]� DecreaseRepeatButton P� 
SourceName  ButtonSelectBrush  	DarkBrush �� Angle  SliderRepeatButton 	 SliderRepeatButton1 
 SliderThumb 
 Slider  Horizontal_Slider �� Triggers��%x  &	        &	 �     &	 v    &	 &    &	 �    &	 ]    &	 l    &		 �    &	
 *	    &	 
    7 	 7 
 7  &	 �
    7  �� " ��.���  �@�OverridesDefaultStyle . ��  � ��$ 5q���  � �� �� " ��.R��� $��border��$��0q�#��z��._��� $��Center=�$��Center=��� " ��.���  �@�OverridesDefaultStyle . ��  � ��$ 5q���  � �� �	#FFC4B7A4��  � �� �� " ��.R��� $��border��$��0q�#��z��._��� $��Center=�$
��0,12q��� " ��.���  �@�OverridesDefaultStyle . ��  � �� �� " ��.R��� $��border��$��1q����	#ffC4B7A4$ 0,0,0,3q�._��� $��Center=�$��Center=��� " ��.���  �@�OverridesDefaultStyle . ��  � �� �� " ��.R��� $��border��$��1q����	#ff64524c$ 0,0,0,3q�._��� $��Center=�$��Center=��� " n�.���  � �� �� " n�.R�� $
 Grid��.+��� $��Stretch=�$��Stretch=�$
��Auto��$
��Auto��
������ �� $ CornerScrollBarRectangle��$ 0��$��Stretch=�$��Stretch=�$
��Auto��$
��Auto��$��0,1,0,1q�#��z��	 P� 
 �@�Tag$ 
Horizontal��. ��� $ CornerScrollBarRectangle�� � ��$
 Auto���� $ CornerScrollBarRectangle�� � ��$ 6���� " ��.���  �@��IsFlicksEnabled .  ��  � �� �#64524c��  � ��
 ���� ��  � ��$ 7����  � �� �� " ��.R�� $ GridRoot��$��7��#��z�� �� $��0.00001* �.+�]� $ 
PART_Track��$��0����. |�.   n� $ Thumb��#��z��#���   �� $ PageUp��$��ScrollBar.PageDownCommand��$ 0Q�|�.   �� $ PageDown��$��ScrollBar.PageUpCommand��$ 0Q�|�.  	 P� $ Thumb��
 �@J�IsMouseOver . . ��� # �  $ Thumb�� � ��P� $ Thumb��
 �@n�
IsDragging . . ��� # �  $ Thumb�� � ��P� 
 � {� .  . ��� $ Thumb�� � y�$ 	Collapsed=�P� 
 �@��Orientation$ 
Horizontal=�. ��� $ GridRoot�� �@�LayoutTransform �� $	 -90Q��� $ 
PART_Track�� �@�LayoutTransform �� $	 -90Q���  � ��$
 Auto����  � ��$ 8���� $ Thumb�� �@�Tag$ 
Horizontal���� $ PageDown�� � ��$ ScrollBar.PageLeftCommand���� $ PageUp�� � ��$  ScrollBar.PageRightCommand���� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . �� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . �� " n�.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � �� �� " n�.R��� $��16��$��45��
������ $��Center=�$	��Top=�.���� $��2��$��45�����	#FFC4B7A4$��-5q��� " ��.R��  �� $
��Auto ��� $
��Auto �#��z���� $
��Auto �.+�]� $ 
PART_Track��$��1�� �� 8��  $��Slider.DecreaseLarge�� n� 8��  �� 8�� $��Slider.IncreaseLarge���� " ��.���  �@�OverridesDefaultStyle . ��  �@J�SnapsToDevicePixels . ��  �@��IsMoveToPointEnabled . ��  �@�LargeChange$ 1Q� P� 
 �@��Orientation$ 
Horizontal=�. ���  � ��$ 21����  � ��$	 104����  � ��8   