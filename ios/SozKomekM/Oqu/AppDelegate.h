//
//  AppDelegate.h
//  Oqu
//
//  Created by Ельнар Шопанов on 21.02.16.
//  Copyright © 2016 Ельнар Шопанов. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface AppDelegate : UIResponder <UIApplicationDelegate, UITabBarControllerDelegate>

@property (strong, nonatomic) UIWindow *window;
@property (weak, nonatomic) IBOutlet UITabBarController *tabBarController;

@end