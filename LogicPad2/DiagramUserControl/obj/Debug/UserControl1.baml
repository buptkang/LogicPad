   M S B A M L   `   `   `  ����M  IDiagramUserControl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:LogicPad2.DiagramLogicPad2.Diagram  5      $    LogicPad2.Diagram.UserControl1   5      W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      8x,http://schemas.microsoft.com/winfx/2006/xaml    5      Hmc;http://schemas.openxmlformats.org/markup-compatibility/2006    5      >d2http://schemas.microsoft.com/expression/blend/2008    5      /localclr-namespace:LogicPad2.Diagram    5        � 	Resources  5	         TButtonStyle �� 
TargetType �� BasedOn �� Property �� Value �� Triggers �� Binding �� RelativeSource �� Path	 �� Value��%�   &	        7 ��� 5
   
   " ��6%   8   .�5      ��  �@^�ShowOnDisabled . 6>    5      �� 5       6   ��  �� *Self+$ 	IsEnabled�		 False6g   .a�5      ��  �@J�Opacity$
 0.50Q�60   5      5      5      ��5      �� 5   
   
 �� ImageSource$
 Images/metal.jpg��6    j� Stretch$
 None=�64    j� TileMode$
 Tile=�6C    j� Viewport$ 1,1,211,211�6S    j� ViewportUnits$ Absolute=�6j   z�5      V� 5   
   .~�5      �� -   5        UserControlScaleXY�� �� ScaleX$ 1Q�5   9    �� ScaleY$ 1Q�6D   T� 5      -   5        UserControlXY�� T� X$ 0Q�5   8    T� Y$ 0Q�6>   5      .��5       �� -   5         ��Name$ controlBorder��5       $��1q�6$   
���   �._�5"   
   � -   5         �Name$ Grid0��5"       � RowDefinitions 5$      �� 5%      $
��Auto �6    $	��450��6.   �� 5&      $��28 �6     � ColumnDefinitions 5)      �� 5*      $	��600��6#   .+�5-      �� $��1q�6   
���   �$��0��6=   ._�5.      �� $
��Auto=�6   $
��Auto=�5/      .��50      � -   5        $��Grid1��50       51      �� 52      $
��Auto �6,   �� 53      $��* �6,    55      �� 56      $
��Auto �6/   �� 57      $��* �6/   .+�5:      %   LogicPad2.Diagram.GateInkCanvas  -   5          Name$ 	inkCanvas��5:   .   $��1��6A   $��1��6N   $��2��6^   +   %LogicPad2.Diagram.PieMenuGateSelector  5<      -   5          Name$ pieMenuGateSelector��5<   4   $��1��6Q   $��1��6^   $y�	Collapsed=�6n   $��2��6�   �� 5>      -   5         ��Name$ adornerLayer��5>   !   $y�	Collapsed=�67   5?      5@      5A      �� 5C      $��1��6   $��1q�6"   
���   �._�5D      �� -	   5        $��controlRegion��5D       �� Orientation$ 
Horizontal=�5E      $��Right=�5F      $	��Top=�69   .+�5H      �� $��Logic Diagram��6   $
��Bold"�64   $��Italic$�6F    J� Opacity$	 0.7Q�6Y   �� 5J      -
   5         ��Name$ btnSave��5J   I   $��
170,0,10,0q�6    � ToolTip$ Save As Image��61   $	 0.5Q�6Z   .��5K      �� $��Images/saveAsHS.png��6    5L      �� 5N      -   5        $ 	btnCancel��5N   I   $��0,0,10,0q�6   $ Close InkCanvas��6/   $	 0.5Q�6\   .��5O      �� $��Images/cancel.png��6    5P      �� 5R      -   5        $ btnTransform��5R   G   $��0,0,0,0q�6   $ Move InkCanvas��6.   $	 0.5Q�6]   .��5S      �� $��Images/hand_tool.png��6    5T      �� 5V      -   5        $ btnScale��5V   I   $��10,0,0,0q�6   $ Scale InkCanvas��6/   $	 0.5Q�6[   .��5W      �� $��Images/Scale.png��6    5X      5Z      5[      5]      5_      5`      