   M S B A M L   `   `   `  ����  iMineLauncher(    "iMine.Launcher.Windows.LoginWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	     � Title$  iMine Логин��5   	    � ShowInTaskbar .  $	��400��63   $	��306��6@    � WindowStyle$
 None=�6L    � AllowsTransparency . $y�Visible=�6y   ���	#EE191511 � 
ResizeMode$ NoResize=�6�   .��5      (� $
��Left=�6   .��5	   
   � $��Center=�6   $	��Top=�6,   $	��400��6D   $	��306��6Q   .+�5
      �� $��
ЛОГИН��6   $��Center=�6%   ���#d0b499$	��Top=�6W   $��0,22,0,0q�6o   $
��Bold"�6�   $��18(�6�   �� 5      -   5         ��Name$ CloseButton��5         StartMenuButtons#���   6*   $��Right=�6U   $	��Top=�6q   $��28��6�   $��28��6�   $��	0,16,12,0q�6�    �� IsCancel . 6�   �� 5      -   5         ��Name$ ErrorBox��5      $y�Hidden=�6*   ���	#33FF0000
���  ��$
��Left=�6f   $��	20,76,0,0q�6�   $	��Top=�6�   $	��274��6�   �� 5      -   5        $ ResetPassButton��5   <   $��3��6/   $
��Left=�6�   $	��Top=�6�   $	��126��6�   $��63��6�     BlackButtons#���  6�   $��
25,241,0,0q�6  ��5      �� 5      $ ��Забыли пароль?��6   ���	#FFC4B7A4$��12(�6L   $	��Top=�6Z   $��0,3,0,0q�6r   $
��Left=�6�   $��Center=�6�   $��Center=�6�   5      �� 5      -   5        $ NoAccountButton��5   >   $��4��61   $��Right=�6�   $	��Top=�6�   $	��126��6�   $��63��6�   #���  6�   $��
0,241,25,0q�6  ��5      �� 5      $��Регистрация��6   ���	#FFC4B7A4$��12(�6I   $	��Top=�6W   $��0,3,0,0q�6o   $
��Left=�6�   $��Center=�6�   $��Center=�6�   5      �� 5      $
��Left=�6   $��	35,92,0,0q�63   $
��Wrap=�6F   $��
Логин��6Z   $��14(�6g   ���	#FF563f35$	��Top=�6�    J� RenderTransformOrigin$ 
0.74,0.609&�6�   �� 5      $
��Left=�6   $��
35,166,0,0q�63   $
��Wrap=�6G   $��Пароль��6[   $��14(�6i   ���	#FF563f35$	��Top=�6�   �� 5      $��35,116,35,250q�6   $��2q�6,   	 J� SnapsToDevicePixels	 . ���	#FF8e806e._�5      �� $��2q�6   $��-1q�6-   ���	#FFb6a18b._�5      �� -   5        
 ��Name$
 LoginBox��5   +   $��0��6   $��-1q�6=    �� 	MaxLength$ 16��6I   $��0q�6X   ���	#FFb6a18b$��Center=�6�   $��Center=�6�   $��15(�6�   ���	#FF563f35.��6�   ��5      �� 5       �� EndPoint$ 0.5,1&�62    �� 
StartPoint$ 0.5,0&�6C   .��5   "   �  � Color$ 	#FF8B7764��6/    � Offset$ 0Q�6A   � 5   "   $ 	#FFE5D8C5��6/   $ 1Q�6A   5       .��5!   .   5"      5#      5$      �� 5%      $��35,190,35,176q�6   $��2q�6,   	 . ���	#FF8e806e._�5&      �� $��2q�6   $��-1q�6-   ���	#FFb6a18b._�5'      G� -   5         G�Name$ PasswordBox��5'   /   $��1��6"   $��-1q�6D   $��0q�6P   ���	#FFb6a18b$��Center=�6|   $��Center=�6�   $��15(�6�   ���	#FF563f35��5(      �� 5)      $ 0.5,1&�62   $ 0.5,0&�6C   .��5*   "   � $ 	#FF8B7764��6/   $ 0Q�6A   � 5+   "   $ 	#FFE5D8C5��6/   $ 1Q�6A   5,      5.      5/      50      �� 51      -   5        $ LoginButton��51   &    �� 	IsDefault . $��2��6;   $	��172��6a   $��86��6m   $��67,304,67,10q�6y   #���   6�   ��52      �� 53      $��
Войти��6   
�������$��18(�6?   $��	0,-18,0,0q�6M   55      56      57      58      