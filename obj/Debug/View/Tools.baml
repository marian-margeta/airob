   M S B A M L   `   `   `  ����@  <Airob, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:Airob.View
Airob.View  5      3clr-namespace:Airob.ConvertersAirob.Converters  5	      /clr-namespace:Airob.ServicesAirob.Services  5
          Airob.View.Tools   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5      f bGalaSoft.MvvmLight.Platform, Version=5.3.0.19032, Culture=neutral, PublicKeyToken=5f873c45e98af8a1(c http://www.galasoft.ch/mvvmlight  5      $localclr-namespace:Airob.View  5      /
convertersclr-namespace:Airob.Converters  5	      +servicesclr-namespace:Airob.Services  5
        � DataContext  5      �� *Main+   Locator �� Source# [   � 	Resources 5       �� 
TargetType �� BasedOn �� Property �� Value*  $Airob.Converters.EnumToBoolConverter  EnumToBoolConverter��%�   '	�       7d�&	 �     �� 5   
   " 	�6   8   .�5      ��  � �� 6,   Z� �� 5       � ��
 ���� 6+   �� 5       � ��$ 0q�60   �� 5       � ��$	 4,5q�6(   5        5   
   6)   5      .��5      � .+�5   
   �� 
�������$��2q�6)    �� CornerRadius$ 5��6=   ���	#FFEAEAEA$	��Top=�6e   $��3q�6}   ._�5      �� $	��Top=�6    �� Orientation$ Vertical=�61   $��0q�6H   .+�5      	� 	 � ToolTip$	 Move objects��6   
 d� 	IsChecked
 5      ��  �� Path$ AppState.EditState� �� 	Converter# [    Airob.Services.EditState   Motion �� ConverterParameter# Z .��5      F� $��18��6    �� Stretch$ Uniform=�6&   
���   �����p���z6��P@���� c������ߧ� �J�ߧ� �J��Y�4S��Y�z6��P@�j�  �>� c���Y�4ߧ��Y�4ߧ�ߧ���Y�ߧ���Y c�Q    �z6��P@���Y�S���Y �J�4ߧ �J�4ߧ���� c������z6��P@���4S���� �J���� �J �J���� �J�����SQ���z6��P@6E   5       	� 5!      $	 Rotation of the robot��6   
 5"      �� $ AppState.EditState�# [    Rotation# Z .��5%      F� $��16��6   $ Uniform=�6&   
���   ������ �3 *Ә�2h�;P '�AX�N\)o�Ȝ� �\� ������  Ւ�j>� ����{ �X�d \)oӬ�M �AX��A �;� A  *���A 0#� t�M �� �Md � Ӥ�z e� ���  p� �� �7� �l�� ����Ic��~ Q��� �j�   ?�(2� 7_ ��Q� R_ ȳ� �c Xr 5m � �[ >nv ��G ��� ,6 :�� Ӕ� ̵� ��t�7�?���  *��8C��?x?�l2�� XaJ� �a�Tb 8�w&�$ �,6 ����v�G tg�J'\ 4ۼ��r L��pt� d/�i� L����� ����L�� L����� d/���L��`04ۼ�f-tg��,?���Ӹ�P���]8�w �f�a�L�oXaJ �t�l2��t *� �3 *6E   5&      	� 5'      $	 Add new points to line��6   
 5(      �� $ AppState.EditState�# [    AddPoint# Z .��5,      � .+�5-      F� $��16��6   $ Uniform=�6*    � UseLayoutRounding .  
���   �������}�(?��    (?��    ɬѰ���� ь@�`����}�(?���d(�(�$8��{�0�!2� ��|t\W8 ��|t\W8 �٨�
���QI��NP@    ��\W8 ��\W8 Ѡ�d�O�Ӡ�d�O���Xڷ����Ӝ�S���d(�(��d(�(6c   5.      5/      	� 50      $	 Remove line��6   .  (System.Windows.Interactivity.Interaction   Triggers 51      /  )System.Windows.Interactivity.EventTrigger  52         	EventName$ Checked��6)   .  (System.Windows.Interactivity.TriggerBase   Actions. 53      /  )GalaSoft.MvvmLight.Command.EventToCommand     Command 6/   �� *RemoveCommand+54      .��56      F� $��15��6   $ Uniform=�6&    .  
���   ������@�T�[ Ә(6T�[  �JZp  �JX� � �J��� �(6�� @��� ���- �� �2} �� ��� X� �    Zp 2} T�[ ��- T�[ р�T�[ р���- Ӏ�ڄ �%'A_p�  P?@o@A_p�  P?Q�2
A_p�  P?S�#A_p�  P?��7ڄ ��7��- р�7T�[ �@�T�[ �P}��JQ��7��A϶P@�H�sh�7ј�7��s�P}��T%� Q% ���� a�T%ј���s����h�7р��)��� a�J�M%�u��P}��J�[ hN]Ѭ�[ h��-���P@h��-���P@hN]Ѭ�[ hN]6_   57      	� 58      $��30��59      $��30��6    $	 Add barrier��5:      
 5;      �� $ AppState.EditState�# [    
AddBarrier# Z .��5?      �� $	��910��6   $	��910��6)   z�5@      V� 5A      .~�5B   "   ��  �� ScaleX$ 0.018Q�61    �� ScaleY$ 0.018Q�6@   T� 5C   "    T� X$ 3Q�65   5D      .+�5F      F�  .  
���   �������In
@��,�@��	@��,�@��	�\�@��\�@�@��,��M�@��,� u@��,� ��2k-� �@.�� ����.�� ����/ u��0�M���0��In
��0Ӏ"��0�v����/�v����.��v�@.��v��2k-�"@��,�In
@��,	5G      F� 5H       .  
���   ��������2@��,� b2@��,� b2�\� �-�\� �-@��,р:,@��,��,�+@��,��+�2k-��+@.р�+���.Ӏ�+���/�,�+��0�:,��0р�2��0�@ڎ3��0�.4���/�.4���.р.4@.Ӏ.4�2k-@ڎ3@��,��2@��,	5I      � 5J       .  
���   � � PointsO �   @
G��# �C��#����#��� �& �� <� �� <�@
G �&6I   � 5K       .  
���   �; �   `�S#��#�e���#���! <� ^�) <�`�S# �&6I   F� 5L       .  
���   �f������8��#���5��#� � ��#�Z��3M�    ���    @�S��J @u��y�  <��୎ <�р�8��#	5M      F� 5N       .  
���   ������ m5��#��=�*��#� �1 <��������@ <��@i�5@u������o�@@�S�����o�@��������o�@�3MӀ=6�Z��E�5��# m5��#	5O      5P      5Q      �� 5R      $��0q�6   	� 5S      $	 Run simulation��6   $��30��67   
 5T      �� $ AppState.EditState�# [    
Simulation# Z .��5W      � $��13��6   $ Uniform=�6)    .  
���   � �   ��� �pr ��� 6b   5X      5Y      5Z      5[      5\      