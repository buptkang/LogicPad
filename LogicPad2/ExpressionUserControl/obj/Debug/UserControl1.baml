   M S B A M L   `   `   `  ����E  AInq, Version=1.0.4287.38769, Culture=neutral, PublicKeyToken=null<)clr-namespace:starPadSDK.Inq;assembly=InqstarPadSDK.Inq  5      B >WPFHelp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullH1clr-namespace:starPadSDK.WPFHelp;assembly=WPFHelpstarPadSDK.WPFHelp 5       ExpressionUserControl'   !LogicPad2.Expression.UserControl1   5      W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      8x,http://schemas.microsoft.com/winfx/2006/xaml    5      Hmc;http://schemas.openxmlformats.org/markup-compatibility/2006    5      >d2http://schemas.microsoft.com/expression/blend/2008    5      BStarPadSDK_Inq)clr-namespace:starPadSDK.Inq;assembly=Inq    5      CWPFHelp1clr-namespace:starPadSDK.WPFHelp;assembly=WPFHelp    5      -   5          >�Name$  expressionUserControl��5      ��5
      �� 5   
    �� ImageSource$ Images/metal.jpg��6    j� Stretch$
 None=�64    j� TileMode$
 Tile=�6C    j� Viewport$ 1,1,211,211�5       j� ViewportUnits$ Absolute=�6,   z�5      V� 5   
   .~�5      �� -   5        UserControlScaleXY�� �� ScaleX$ 1Q�5   9    �� ScaleY$ 1Q�6D   T� 5      -   5        UserControlXY�� T� X$ 0Q�5   8   	 T� Y$	 0Q�6>   5      .��5      �� -   5        
 ��Name$
 controlBorder��5      $��1q�6%   
���   �._�5   
   � -   5         �Name$ Grid0��5       � RowDefinitions 5      �� 5      $
��Auto �6    �� 5      $��28 �6    .+�5      ]� $��0��6   .+�5!      `� -   5         `�Name$ alternatesTray��5!   P   $��Right=�6"    `� Orientation$ Vertical=�69   .�5"      c� -   5         c�Name$ alternatesMenu��5"   "   $��Times New Roman)�6:   5#      �� 5%      -   5        $��
GCScroller��5%      $
��Auto=�61   $
��Auto=�6T   .��5'      � }�. .+�5(      ,  &starPadSDK.WPFHelp.ContainerVisualHost  -	   5          Name$ underlay��5(   k   $��Stretch=�6M   $
��Auto��6}    J� OpacityMask 66   Z� 6}      starPadSDK.Inq.InqCanvas  5)      -
   5          Name$ 	inqCanvas��5)   3   ���	#00000000$	��200��6]   5*      5+      5,      �� 5.      $��1��6   $��1q�6"   
���   �._�5/      �� -   5        $��controlRegion��5/       �� Orientation$ 
Horizontal=�50      $��Right=�51      $	��Top=�69   .+�53      �� $��Logic Expression��6   $
��Bold"�67   $��Italic$�6I    J� Opacity$	 0.7Q�6\   �� 55      -   5         ��Name$ btnSave��55   I   $��
170,0,10,0q�6    � ToolTip$ Save As Image��61   $	 0.5Q�6Z   .��56      �� $��Images/saveAsHS.png��6    57      �� 59      -   5        $ 	btnCancel��59   I   $��0,0,10,0q�6   $ Close InkCanvas��6/   $	 0.5Q�6\   .��5:      �� $��Images/cancel.png��6    5;      �� 5=      -   5        $ btnTransform��5=   G   $��0,0,0,0q�6   $ Move InkCanvas��6.   $	 0.5Q�6]   .��5>      �� $��Images/hand_tool.png��6    5?      �� 5A      -   5        $ btnScale��5A   I   $��10,0,0,0q�6   $ Scale InkCanvas��6/   $	 0.5Q�6[   .��5B      �� $��Images/Scale.png��6    5C      5E      5F      5H      5J      5L      