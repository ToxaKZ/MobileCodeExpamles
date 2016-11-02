//
//  DatabaseHelper.h
//  Oqu
//
//  Created by Ельнар Шопанов on 27.02.16.
//  Copyright © 2016 Ельнар Шопанов. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <sqlite3.h>

@class HistoryModel;
@class FavoriteModel;

@interface DatabaseHelper : NSObject {
    sqlite3 *_db;
    NSMutableArray *_availableTableNames;
}

+ (nonnull DatabaseHelper*)instance;
- (nullable NSArray*)searchWordsByText:(nonnull NSString*)text;
- (nullable NSString*)prepareTextForSearch:(nonnull NSString*)text;
- (nullable NSArray*)articlesWithWordId:(NSInteger)wordId;
- (int)wordIdWithText:(nonnull NSString*)text;
- (nullable NSMutableArray*)history;
- (void)insertHistory:(nonnull HistoryModel*)history;
- (void)deleteHistory:(nonnull HistoryModel*)history;
- (void)clearHistory;
- (nullable NSMutableArray*)favorites;
- (void)insertFavorite:(nonnull FavoriteModel*)favorite;
- (void)deleteFavorite:(nonnull FavoriteModel*)favorite;
- (void)clearFavorites;

@end