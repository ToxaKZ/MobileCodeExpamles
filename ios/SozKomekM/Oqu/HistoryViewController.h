//
//  SecondViewController.h
//  Oqu
//
//  Created by Ельнар Шопанов on 21.02.16.
//  Copyright © 2016 Ельнар Шопанов. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface HistoryViewController : UIViewController <UITableViewDelegate, UIAlertViewDelegate> {
    NSMutableArray *_dataSet;
}

@property (weak, nonatomic) IBOutlet UITableView *historyTableView;

- (void)refresh;

@end