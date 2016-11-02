//
//  SecondViewController.m
//  Oqu
//
//  Created by Ельнар Шопанов on 21.02.16.
//  Copyright © 2016 Ельнар Шопанов. All rights reserved.
//

#import "HistoryViewController.h"
#import "HistoryModel.h"
#import "DatabaseHelper.h"
#import "ExceptionHelper.h"
#import "Constants.h"
#import "ArticleViewController.h"

@implementation HistoryViewController

@synthesize historyTableView;

- (void)viewDidLoad {
    [super viewDidLoad];
    
    [self.historyTableView registerClass:[UITableViewCell self] forCellReuseIdentifier:IDENTIFIER_CELL];
    self.navigationItem.rightBarButtonItem = self.editButtonItem;

    [self refresh];
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    return _dataSet != nil ? _dataSet.count : 0;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:IDENTIFIER_CELL forIndexPath:indexPath];
    
    HistoryModel *model = [_dataSet objectAtIndex:indexPath.row];
    cell.textLabel.text = model.name;
    
    return cell;
}

- (void)refresh {
    _dataSet = [[DatabaseHelper instance] history];
    [historyTableView reloadData];
    [self afterRefresh];
}

- (void)afterRefresh {
    self.editing = NO;
    self.editButtonItem.enabled = _dataSet.count > 0;
}

- (void)setEditing:(BOOL)editing animated:(BOOL)animated {
    [super setEditing:editing animated:animated];
    
    @try {
        [historyTableView setEditing:editing animated:animated];
        [self updateState];
    }
    @catch (NSException *e) {
        [[ExceptionHelper sharedInstance] alertWithException:e];
    }
}

- (void)tableView:(UITableView *)tableView commitEditingStyle:(UITableViewCellEditingStyle)editingStyle forRowAtIndexPath:(NSIndexPath *)indexPath {
    @try {
        if (editingStyle == UITableViewCellEditingStyleDelete) {
            HistoryModel *model = [_dataSet objectAtIndex:indexPath.row];
            [[DatabaseHelper instance] deleteHistory:model];
            [_dataSet removeObjectAtIndex:indexPath.row];
            [tableView deleteRowsAtIndexPaths:[NSArray arrayWithObject:indexPath] withRowAnimation:UITableViewRowAnimationFade];
            
            if (_dataSet.count == 0) {
                [self afterRefresh];
            }
        }
    }
    @catch (NSException *e) {
        [[ExceptionHelper sharedInstance] alertWithException:e];
    }
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath {
    @try {
        HistoryModel *model = [_dataSet objectAtIndex:indexPath.row];
        
        if (model != nil) {
            [ArticleViewController showWithParent:self wordID:model.wordId wordName:model.name history:NO];
        }
    }
    @catch (NSException *e) {
        [[ExceptionHelper sharedInstance] alertWithException:e];
    }
}

- (void)updateState {
    if (historyTableView.isEditing) {
        self.navigationItem.leftBarButtonItem = [[UIBarButtonItem alloc] initWithTitle:NSLocalizedString(RES_KEY_CLEAR, nil) style:UIBarButtonItemStyleBordered target:self action:@selector(clearButtonClick:)];
    } else {
        self.navigationItem.leftBarButtonItem = nil;
    }
}

- (void)clearButtonClick:(id)sender {
    @try {
        UIAlertView *alert = [[UIAlertView alloc] initWithTitle:nil message:NSLocalizedString(RES_KEY_QUESTION_CLEAR_HISTORY, nil) delegate:self cancelButtonTitle:NSLocalizedString(RES_KEY_NO, nil) otherButtonTitles:NSLocalizedString(RES_KEY_YES, nil), nil];
        [alert show];
    }
    @catch (NSException *e) {
        [[ExceptionHelper sharedInstance] alertWithException:e];
    }
}

- (void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex {
    @try {
        if (buttonIndex == 1) {
            [[DatabaseHelper instance] clearHistory];
            [self refresh];
        }
    }
    @catch (NSException *e) {
        [[ExceptionHelper sharedInstance] alertWithException:e];
    }
}

@end